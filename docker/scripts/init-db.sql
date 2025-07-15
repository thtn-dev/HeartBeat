-- Database Initialization Script for Social Network Platform
-- This script sets up essential extensions, functions, and optimizations

\echo 'Starting database initialization for Social Network Platform...'

-- =============================================================================
-- EXTENSIONS SETUP
-- =============================================================================

-- Enable UUID generation (even though we're using sequential IDs, 
-- UUIDs might be useful for external references, tokens, etc.)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Enable pg_stat_statements for query performance monitoring
-- This extension tracks execution statistics of all SQL statements
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- Enable pg_trgm for fuzzy text search and similarity matching
-- Essential for username/display name search functionality
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- Enable unaccent for accent-insensitive text search
-- Important for international user base
CREATE EXTENSION IF NOT EXISTS unaccent;

-- Enable btree_gin for improved indexing on composite types
-- Useful for complex queries involving multiple columns
CREATE EXTENSION IF NOT EXISTS btree_gin;

\echo 'Extensions created successfully.'

-- =============================================================================
-- CUSTOM FUNCTIONS FOR SOCIAL NETWORK OPTIMIZATION
-- =============================================================================

-- Function to update the 'updated_at' timestamp automatically
-- This will be used as a trigger function for audit trail
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Function to generate human-readable IDs (for public facing IDs)
-- Example: user handles, post IDs that are shareable
CREATE OR REPLACE FUNCTION generate_short_id()
RETURNS TEXT AS $$
DECLARE
    characters TEXT := 'ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789';
    result TEXT := '';
    i INTEGER;
BEGIN
    FOR i IN 1..8 LOOP
        result := result || substr(characters, floor(random() * length(characters) + 1)::INT, 1);
    END LOOP;
    RETURN result;
END;
$$ LANGUAGE plpgsql;

-- Function for case-insensitive, accent-insensitive username search
-- This is crucial for user discovery in social networks
CREATE OR REPLACE FUNCTION normalize_text(input_text TEXT)
RETURNS TEXT AS $$
BEGIN
    RETURN lower(unaccent(trim(input_text)));
END;
$$ LANGUAGE plpgsql IMMUTABLE;

\echo 'Custom functions created successfully.'

-- =============================================================================
-- PERFORMANCE OPTIMIZATION SETTINGS
-- =============================================================================

-- Set default statistics target for better query planning
-- This affects how much statistical information is collected about table columns
ALTER DATABASE socialx_db SET default_statistics_target = 100;

-- Set work_mem for this database to optimize sorting and hashing operations
-- Social networks do lots of timeline sorting and user matching
ALTER DATABASE socialx_db SET work_mem = '16MB';

-- Enable JIT compilation for complex queries (PostgreSQL 11+)
-- This can significantly speed up analytical queries
ALTER DATABASE socialx_db SET jit = on;

-- Set random_page_cost to favor index usage on SSDs
-- Social networks benefit heavily from index usage for user/content lookups
ALTER DATABASE socialx_db SET random_page_cost = 1.1;

\echo 'Database-level optimizations applied successfully.'

-- =============================================================================
-- MONITORING AND MAINTENANCE SETUP
-- =============================================================================

-- Create a view for monitoring slow queries
-- This helps identify performance bottlenecks in production
CREATE OR REPLACE VIEW slow_queries AS
SELECT 
    query,
    calls,
    total_exec_time,
    mean_exec_time,
    max_exec_time,
    rows,
    100.0 * shared_blks_hit / nullif(shared_blks_hit + shared_blks_read, 0) AS hit_percent
FROM pg_stat_statements 
WHERE mean_exec_time > 100  -- Queries taking more than 100ms on average
ORDER BY mean_exec_time DESC;

-- Create a view for monitoring database activity
CREATE OR REPLACE VIEW db_activity AS
SELECT 
    datname,
    numbackends as active_connections,
    xact_commit as transactions_committed,
    xact_rollback as transactions_rolled_back,
    blks_read as blocks_read,
    blks_hit as blocks_hit,
    100.0 * blks_hit / nullif(blks_hit + blks_read, 0) AS cache_hit_ratio,
    tup_returned as tuples_returned,
    tup_fetched as tuples_fetched,
    tup_inserted as tuples_inserted,
    tup_updated as tuples_updated,
    tup_deleted as tuples_deleted
FROM pg_stat_database 
WHERE datname = 'socialx_db';

-- Create a view for monitoring table sizes and bloat
CREATE OR REPLACE VIEW table_sizes AS
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as total_size,
    pg_size_pretty(pg_relation_size(schemaname||'.'||tablename)) as table_size,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename) - pg_relation_size(schemaname||'.'||tablename)) as index_size
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;

\echo 'Monitoring views created successfully.'

-- =============================================================================
-- ROLE AND SECURITY SETUP
-- =============================================================================

-- Create application-specific roles with minimal required permissions
-- This follows the principle of least privilege

-- Read-only role for reporting and analytics
CREATE ROLE socialx_readonly;
GRANT CONNECT ON DATABASE socialx_db TO socialx_readonly;
GRANT USAGE ON SCHEMA public TO socialx_readonly;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO socialx_readonly;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO socialx_readonly;

-- Read-write role for application operations
CREATE ROLE socialx_readwrite;
GRANT CONNECT ON DATABASE socialx_db TO socialx_readwrite;
GRANT USAGE ON SCHEMA public TO socialx_readwrite;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO socialx_readwrite;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO socialx_readwrite;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO socialx_readwrite;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO socialx_readwrite;

-- Grant appropriate role to the application user
GRANT socialx_readwrite TO socialx_user;

\echo 'Security roles configured successfully.'

-- =============================================================================
-- INITIAL CONFIGURATION VERIFICATION
-- =============================================================================

-- Show current configuration
\echo 'Current database configuration:'
SELECT name, setting, unit, short_desc 
FROM pg_settings 
WHERE name IN (
    'max_connections',
    'shared_buffers', 
    'effective_cache_size',
    'work_mem',
    'random_page_cost'
);

-- Show enabled extensions
\echo 'Enabled extensions:'
SELECT extname, extversion 
FROM pg_extension 
WHERE extname NOT IN ('plpgsql');

\echo 'Database initialization completed successfully!'
\echo 'Ready for Entity Framework Core integration.'