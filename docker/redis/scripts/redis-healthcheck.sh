#!/bin/bash
# docker/redis/scripts/redis-healthcheck.sh

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
REDIS_HOST="${REDIS_HOST:-localhost}"
REDIS_PORT="${REDIS_PORT:-6379}"
TIMEOUT="${TIMEOUT:-5}"

echo "🔍 Redis Health Check Starting..."
echo "Host: $REDIS_HOST:$REDIS_PORT"
echo "Timeout: ${TIMEOUT}s"
echo "----------------------------------------"

# Function to run Redis command
run_redis_cmd() {
    redis-cli -h "$REDIS_HOST" -p "$REDIS_PORT" --raw "$@" 2>/dev/null
}

# Function to print status
print_status() {
    local status=$1
    local message=$2
    
    if [ "$status" = "OK" ]; then
        echo -e "${GREEN}✓${NC} $message"
    elif [ "$status" = "WARNING" ]; then
        echo -e "${YELLOW}⚠${NC} $message"
    else
        echo -e "${RED}✗${NC} $message"
    fi
}

# Test 1: Basic connectivity
echo "1. Testing basic connectivity..."
if timeout "$TIMEOUT" run_redis_cmd ping > /dev/null; then
    print_status "OK" "Redis is responding to PING"
else
    print_status "ERROR" "Redis is not responding"
    exit 1
fi

# Test 2: Memory usage
echo "2. Checking memory usage..."
MEMORY_INFO=$(run_redis_cmd info memory | grep -E "used_memory_human|maxmemory_human")
USED_MEMORY=$(echo "$MEMORY_INFO" | grep used_memory_human | cut -d: -f2 | tr -d '\r')
MAX_MEMORY=$(echo "$MEMORY_INFO" | grep maxmemory_human | cut -d: -f2 | tr -d '\r')

if [ -n "$USED_MEMORY" ]; then
    print_status "OK" "Memory usage: $USED_MEMORY / $MAX_MEMORY"
else
    print_status "WARNING" "Could not retrieve memory information"
fi

# Test 3: Persistence status
echo "3. Checking persistence..."
PERSISTENCE_INFO=$(run_redis_cmd info persistence | grep -E "aof_enabled|rdb_last_save_time")
AOF_ENABLED=$(echo "$PERSISTENCE_INFO" | grep aof_enabled | cut -d: -f2 | tr -d '\r')
RDB_LAST_SAVE=$(echo "$PERSISTENCE_INFO" | grep rdb_last_save_time | cut -d: -f2 | tr -d '\r')

if [ "$AOF_ENABLED" = "1" ]; then
    print_status "OK" "AOF persistence is enabled"
else
    print_status "WARNING" "AOF persistence is disabled"
fi

if [ -n "$RDB_LAST_SAVE" ] && [ "$RDB_LAST_SAVE" != "0" ]; then
    LAST_SAVE_DATE=$(date -d "@$RDB_LAST_SAVE" 2>/dev/null || echo "Unknown")
    print_status "OK" "RDB last save: $LAST_SAVE_DATE"
else
    print_status "WARNING" "No RDB snapshot found"
fi

# Test 4: Performance check
echo "4. Testing performance..."
start_time=$(date +%s%N)
for i in {1..10}; do
    run_redis_cmd set "health_check_$i" "test_value" > /dev/null
    run_redis_cmd get "health_check_$i" > /dev/null
    run_redis_cmd del "health_check_$i" > /dev/null
done
end_time=$(date +%s%N)

# Calculate average time per operation
total_time=$((end_time - start_time))
avg_time_ms=$((total_time / 30000000))  # 30 operations, convert to ms

if [ "$avg_time_ms" -lt 5 ]; then
    print_status "OK" "Average operation time: ${avg_time_ms}ms (< 5ms target)"
elif [ "$avg_time_ms" -lt 10 ]; then
    print_status "WARNING" "Average operation time: ${avg_time_ms}ms (5-10ms)"
else
    print_status "ERROR" "Average operation time: ${avg_time_ms}ms (> 10ms - performance issue)"
fi

# Test 5: Connection count
echo "5. Checking connections..."
CONNECTED_CLIENTS=$(run_redis_cmd info clients | grep connected_clients | cut -d: -f2 | tr -d '\r')
if [ -n "$CONNECTED_CLIENTS" ]; then
    if [ "$CONNECTED_CLIENTS" -lt 100 ]; then
        print_status "OK" "Connected clients: $CONNECTED_CLIENTS"
    else
        print_status "WARNING" "Connected clients: $CONNECTED_CLIENTS (high)"
    fi
else
    print_status "ERROR" "Could not retrieve client information"
fi

echo "----------------------------------------"
echo "🎉 Health check completed!"

# Exit codes
# 0: All tests passed
# 1: Critical failure (Redis not responding)
# 2: Warning conditions detected

if run_redis_cmd ping > /dev/null; then
    echo -e "${GREEN}Redis is healthy and operational${NC}"
    exit 0
else
    echo -e "${RED}Redis has critical issues${NC}"
    exit 1
fi