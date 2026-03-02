CREATE TABLE IF NOT EXISTS audit.action_definitions (
    id   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code VARCHAR(20)  NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL
);

ALTER TABLE audit.action_logs
    ADD COLUMN IF NOT EXISTS action_definition_id BIGINT REFERENCES audit.action_definitions(id),
    ADD COLUMN IF NOT EXISTS parent_action_log_id BIGINT REFERENCES audit.action_logs(id);
