CREATE TABLE project (
    id uuid PRIMARY KEY,
    name text NOT NULL UNIQUE,
    created_at timestamptz NOT NULL
);

CREATE TABLE app (
    id uuid PRIMARY KEY,
    project_id uuid NOT NULL REFERENCES project (id),
    name text NOT NULL,
    source_kind text NOT NULL CHECK (source_kind IN ('image')),
    image_reference text NOT NULL,
    exposure_port integer CHECK (exposure_port BETWEEN 1 AND 65535),
    exposure_subdomain text,
    memory_limit_bytes bigint NOT NULL CHECK (memory_limit_bytes >= 0),
    cpu_limit_millicores integer NOT NULL CHECK (cpu_limit_millicores >= 0),
    created_at timestamptz NOT NULL,
    removed_at timestamptz,
    CHECK ((exposure_port IS NULL) = (exposure_subdomain IS NULL))
);

CREATE UNIQUE INDEX app_name_live ON app (project_id, name) WHERE removed_at IS NULL;
CREATE UNIQUE INDEX app_subdomain_live ON app (exposure_subdomain) WHERE removed_at IS NULL AND exposure_subdomain IS NOT NULL;

CREATE TABLE release (
    id uuid PRIMARY KEY,
    app_id uuid NOT NULL REFERENCES app (id),
    image_reference text NOT NULL,
    image_digest text,
    trigger text NOT NULL CHECK (trigger IN ('manual')),
    phase text NOT NULL CHECK (phase IN ('queued', 'applying', 'succeeded', 'failed')),
    failure_code text,
    failure_message text,
    created_at timestamptz NOT NULL,
    finished_at timestamptz
);

CREATE INDEX release_app ON release (app_id, id DESC);

CREATE TABLE app_state (
    app_id uuid PRIMARY KEY REFERENCES app (id),
    phase text NOT NULL CHECK (phase IN ('not_deployed', 'deploying', 'running', 'stopped', 'crashed')),
    current_release_id uuid REFERENCES release (id),
    observed_at timestamptz NOT NULL
);
