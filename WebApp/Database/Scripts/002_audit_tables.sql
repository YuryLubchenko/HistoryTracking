CREATE TABLE IF NOT EXISTS action_logs (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    timestamp TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS entity_records (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    action_log_id BIGINT NOT NULL REFERENCES action_logs(id),
    entity_name VARCHAR(255) NOT NULL,
    entity_id BIGINT NOT NULL,
    action_type BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS property_records (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    entity_record_id BIGINT NOT NULL REFERENCES entity_records(id),
    property_name VARCHAR(255) NOT NULL,
    property_type VARCHAR(255) NOT NULL,
    old_value JSONB,
    new_value JSONB
);

CREATE INDEX IF NOT EXISTS idx_entity_records_action_log_id ON entity_records(action_log_id);
CREATE INDEX IF NOT EXISTS idx_entity_records_entity ON entity_records(entity_name, entity_id);
CREATE INDEX IF NOT EXISTS idx_property_records_entity_record_id ON property_records(entity_record_id);
