CREATE TABLE IF NOT EXISTS audit.action_logs (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    timestamp TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS audit.entity_types (
    id   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS audit.property_definitions (
    id             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    entity_type_id BIGINT NOT NULL REFERENCES audit.entity_types(id),
    property_name  VARCHAR(255) NOT NULL,
    property_type  VARCHAR(255) NOT NULL,
    UNIQUE(entity_type_id, property_name, property_type)
);

CREATE TABLE IF NOT EXISTS audit.entity_records (
    id             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    action_log_id  BIGINT NOT NULL REFERENCES audit.action_logs(id),
    entity_type_id BIGINT NOT NULL REFERENCES audit.entity_types(id),
    entity_id      BIGINT NOT NULL,
    action_type    BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS audit.property_records (
    id                     BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    entity_record_id       BIGINT NOT NULL REFERENCES audit.entity_records(id),
    property_definition_id BIGINT NOT NULL REFERENCES audit.property_definitions(id),
    old_value              TEXT,
    new_value              TEXT
);

CREATE INDEX IF NOT EXISTS idx_entity_records_action_log_id ON audit.entity_records(action_log_id);
CREATE INDEX IF NOT EXISTS idx_entity_records_entity ON audit.entity_records(entity_type_id, entity_id);
CREATE INDEX IF NOT EXISTS idx_property_records_entity_record_id ON audit.property_records(entity_record_id);
