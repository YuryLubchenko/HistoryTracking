create table if not exists hist_action_types (
    id   bigint primary key,
    name text not null
);

INSERT INTO hist_action_types (id, name) VALUES
    (1, 'CreateClient'),
    (2, 'UpdateClient'),
    (3, 'DeleteClient'),
    (4, 'CreateContact'),
    (5, 'UpdateContact'),
    (6, 'DeleteContact'),
    (7, 'DisableClient'),
    (100, 'SomeAction')
ON CONFLICT (id) DO NOTHING;

CREATE TABLE IF NOT EXISTS hist_action_logs (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    parent_action_log_id bigint references hist_action_logs(id) on delete cascade on update cascade,
    client_id bigint not null,
    action_type_id bigint references hist_action_types(id) on delete no action on update no action,
    timestamp TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS hist_entity_definitions (
    id   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS hist_property_definitions (
    id             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    entity_definition_id BIGINT NOT NULL REFERENCES hist_entity_definitions(id),
    property_name  VARCHAR(255) NOT NULL,
    property_type  VARCHAR(255) NOT NULL,
    UNIQUE(entity_definition_id, property_name, property_type)
);

CREATE TABLE IF NOT EXISTS hist_entity_records (
    id             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    action_log_id  BIGINT NOT NULL REFERENCES hist_action_logs(id),
    entity_definition_id BIGINT NOT NULL REFERENCES hist_entity_definitions(id),
    entity_id      BIGINT NOT NULL,
    action_type    BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS hist_property_records (
    id                     BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    entity_record_id       BIGINT NOT NULL REFERENCES hist_entity_records(id),
    property_definition_id BIGINT NOT NULL REFERENCES hist_property_definitions(id),
    old_value              TEXT,
    new_value              TEXT
);

create index if not exists idx_action_logs_parent_action_log_id on hist_action_logs(parent_action_log_id);
create index if not exists idx_action_logs_action_type_id on hist_action_logs(action_type_id);
CREATE INDEX IF NOT EXISTS idx_entity_records_action_log_id ON hist_entity_records(action_log_id);
CREATE INDEX IF NOT EXISTS idx_entity_records_entity ON hist_entity_records(entity_definition_id, entity_id);
CREATE INDEX IF NOT EXISTS idx_property_records_entity_record_id ON hist_property_records(entity_record_id);
