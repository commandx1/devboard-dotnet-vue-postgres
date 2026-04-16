# Agent: Database - PostgreSQL + Migration Kurallari

## Core Rules

1. **Migration zorunlu**
   - Schema degisiklikleri migration ile yapilir.
   - Uygulamada otomatik schema create/update yok.
   - EF Core migration komutu ornegi:
     - `dotnet ef migrations add InitialCreate --project backend/DevBoard.Api`
     - `dotnet ef database update --project backend/DevBoard.Api`

2. **Her tablo audit alanlari tasir**
```sql
created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
deleted_at TIMESTAMPTZ
```

3. **Key stratejisi**
- User ve dis dunyaya acilan id'ler: `UUID`
- Internal relation id'leri: `BIGINT GENERATED ALWAYS AS IDENTITY`

4. **Soft delete tercih et**
- Fiziksel delete yerine `deleted_at` set edilir.

## Schema Tasarim Prensipleri

### Normalize Et (MERN aliskanligini kir)
```sql
-- Kotu: text[] ile etiket tutmak
-- ALTER TABLE tasks ADD COLUMN tags TEXT[];

-- Iyi: ayri tablo + relation
CREATE TABLE tags (
  id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name TEXT NOT NULL UNIQUE,
  color TEXT NOT NULL DEFAULT '#2563eb'
);

CREATE TABLE task_tags (
  task_id BIGINT NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
  tag_id BIGINT NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
  PRIMARY KEY (task_id, tag_id)
);
```

### Index Stratejisi
```sql
CREATE INDEX idx_projects_user_id ON projects(user_id);
CREATE INDEX idx_tasks_project_id ON tasks(project_id);
CREATE INDEX idx_tasks_status_active ON tasks(status) WHERE deleted_at IS NULL;
CREATE INDEX idx_time_logs_task_id ON time_logs(task_id);
```

## Core Schema (Referans)

```sql
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE task_status AS ENUM ('TODO', 'IN_PROGRESS', 'DONE', 'ARCHIVED');
CREATE TYPE task_priority AS ENUM ('LOW', 'MEDIUM', 'HIGH', 'URGENT');

CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  email TEXT NOT NULL UNIQUE,
  username TEXT NOT NULL UNIQUE,
  password_hash TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

CREATE TABLE projects (
  id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES users(id),
  name TEXT NOT NULL,
  description TEXT,
  color TEXT NOT NULL DEFAULT '#2563eb',
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

CREATE TABLE tasks (
  id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  project_id BIGINT NOT NULL REFERENCES projects(id),
  title TEXT NOT NULL,
  description TEXT,
  status task_status NOT NULL DEFAULT 'TODO',
  priority task_priority NOT NULL DEFAULT 'MEDIUM',
  due_date DATE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

CREATE TABLE time_logs (
  id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  task_id BIGINT NOT NULL REFERENCES tasks(id),
  user_id UUID NOT NULL REFERENCES users(id),
  started_at TIMESTAMPTZ NOT NULL,
  ended_at TIMESTAMPTZ,
  note TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);
```

## Naming
| Alan | Kural | Ornek |
|------|-------|-------|
| Tablo | snake_case plural | `time_logs` |
| Sutun | snake_case | `project_id` |
| Index | `idx_{table}_{column}` | `idx_tasks_project_id` |
| Migration | artan versiyon | `202604151730_initial.sql` |
