# Introduction

This repository is for AI4Green4Students.

This currently consists of a .NET backend that interacts with a PostgreSQL database.

## Getting Started

## Prerequisites

1. **.NET SDK** `8.x`
   - The backend API is .NET8
1. **Node.js** `>=16.9`
   - `16.9` and newer include **Corepack**
1. **Enable [Corepack](https://nodejs.org/api/corepack.html)**
   - Simply run `corepack enable` in your cli
1. Docker

## Database setup

The application stack interacts with a PostgreSQL Server database, and uses code-first migrations for managing the database schema.

The repository contains a `docker-compose` for the database, so just run `docker-compose up -d` to start it running.

When setting up a new environment, or running a newer version of the codebase if there have been schema changes, you need to run migrations against your database server.

The easiest way is using the dotnet cli:

1. If you haven't already, install the local Entity Framework tooling

   - Anywhere in the repo: `dotnet tool restore`

1. Navigate to the same directory as `AI4Green4Students.csproj`
1. Run migrations:

   - `dotnet ef database update`
   - The above runs against the default local server, using the connection string in `appsettings.Development.json`
   - You can specify a connection string with the `--connection "<connection string>"` option

## Working with JavaScript

This monorepo uses [pnpm](https://pnpm.io) workspaces to manage JS dependencies and scripts.

Basically, where you might normally use `npm` or `yarn`, please use `pnpm` commands instead.

You don't need to install anything special; Corepack will.

A brief [pnpm cheatsheet](#-pnpm-cheatsheet) is provided later in this document.

## 📁 Repository contents

Areas within this repo include:

- Application Source Code
  - .NET8 backend API `AI4Green4Students`
  - Azure Functions App `az-functions`
  - React (Vite) frontend web app `client-app`

## Backend Configuration

Notes on configuration values that can be provided, and their defaults.

The backend app can be configured in any standard way an ASP.NET Core application can. Typically from the Azure Portal (Environment variables) or an `appsettings.json`.

```yaml
OutboundEmail:
  ServiceName: AI4Green4Students
  FromName: No Reply
  FromAddress: noreply@example.com
  ReplyToAddress: ""
  Provider: local

  # If Provider == "local"
  LocalPath: /temp

  # If Provider == "sendgrid"
  SendGridApiKey: ""

  # If Provider == "smtp"
  SmtpHost: "" # SMTP host name
  SmtpPort: # SMTP port
  SmtpUsername: "" # SMTP username
  SmtpPassword: "" # SMTP password
  SmtpSecureSocketEnum: # for example, assign 2 to implement SslOnConnect
  # Secure socket options
  # 1 - Auto
  # 2 - SslOnConnect
  # 3 - StartTls
  # 4 - StartTlsWhenAvailable

  # More information can be found here
  # http://www.mimekit.net/docs/html/T_MailKit_Security_SecureSocketOptions.htm

  ExcludedEmailAddresses: [] # Array of email addresses excluded from receiving emails, typically used for test or dummy emails.
  # Only applies to SMTP email provider for now.
```

# Build a database with PubChem data for local development

### Step 1 - Setup

- Install [Poetry](https://python-poetry.org/docs/#installing-with-the-official-installer)

- Clone the [AI4Green](https://github.com/AI4Green/AI4Green/tree/main) repository.

- Navigate to the project directory. `cd AI4Green`

- Install the project dependencies. `poetry install`

- Activate the virtual env created by Poetry. `poetry shell`

### Step 2 - App Configuration

Configuration is controlled by `PubChemSeeding/sources/config.py`, the default values can be overridden by setting environment variables.

```yaml
DATABASE_URL: postgresql://postgres:example@localhost:5433/ai4green # Postgres database connection string.
COMPOUND_LIMIT: 10000 # Sets the limit for the number of compounds extracted from PubChem. Note that a large database can cause smaller servers to crash.
```

### Step 3 - Download PubChem data and build db

- Navigate to the directory `Webapp`. `cd Webapp`

- Run `flask db upgrade` to make database migrations.

- Run `flask download-pubchem` to download the PubChem database.

- Run `flask seed-db` to seed data from the download.
