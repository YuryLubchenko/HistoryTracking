CREATE TABLE IF NOT EXISTS clients (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    disabled boolean not null
);

create table if not exists contacts (
    id bigint generated always as identity primary key,
    client_id bigint not null references clients(id) on DELETE cascade on update cascade,
    contact_type bigint not null,
    value text not null
)
