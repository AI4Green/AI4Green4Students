# AI4Green4Students

AI4Green4Students (basic summary here).

It consists of a Web application (React frontend, .NET backend API, SQL DB) for users to interact with.

# 👩‍💻 Getting Started

## Prerequisites

1. **.NET SDK** `6.x`
   - The backend API is .NET6 (LTS)
1. **Node.js** `>=16.9` and `<17`
   - `16.9` includes Corepack
   - `16.x` is LTS at time of writing
   - `17` breaks Webpack5 at time of writing
1. **Enable [Corepack](https://nodejs.org/api/corepack.html)**
   - Simply run `corepack enable` in your cli
1. SQL Server DB (localdb is fine)

## Database setup

The application stack interacts with a SQL Server database, and uses code-first migrations for managing the database schema.

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

# 📁 Repository contents

Areas within this repo include:

- Application Source Code
  - .NET6 backend API
  - React (Vite) frontend client app
- Bicep files
  - for describing and deploying entire environment stacks of Azure resources.
- Azure Pipelines
  - Pipelines for building and deploying the applications
- Docs
  - internal documentation for the repo (basically more detailed readmes with focused scopes)

# 💡 pnpm cheatsheet

Most pnpm commands can be done recursively against all workspaces with `-r`

You can target a specific workspace by being inside its workspace directory

- or you can target a workspace by relative directory path `-C <dir>`
- or you can filter workspaces to target using `--filter <filter-spec>`
  - See the docs for more complex filtering than just package name

## Dependency management

To install current dependencies for the whole repo: `pnpm i`

> ℹ
>
> pnpm symlinks `node_modules` inside workspaces.
>
> If you need to clean out `node_modules` you can't just do the root one, so use `pnpm dlx npkill` which will let you delete them all :)

To add a new dependency `pnpm add <package-name>` with `-D` if you want it to be a dev dependency

## Script running

Run scripts with `pnpm <script-name>`

> ℹ
>
> If the name of the script conflicts with a pnpm command, do `pnpm run <script-name>`

# App Configuration

Notes on configuration values that can be provided, and their defaults.

The app can be configured in any standard way an ASP.NET Core application can. Typically from the Azure Portal (Environment variables) or an `appsettings.json`.

````yaml
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

UserAccounts:
  SendEmail: # true or false. if true, sends an email to the user
  GenerateLink: # true or false. if true, generates link to the client
  # the above two options are appicable with account activation (user invite) including resending and changing password.

Registration:
  UseRules: # true or false. If true, checks if email satisfies the registration rules.
  # the above options are curently used in CanRegister method, which determines whether a given email can register or not.

  AllowList: [] # String array containing email/domain that are allowed to register. Example ["@example.com", "allow@example1.com"]
  BlockList: [] # String array containing email/domain that are blocked from registration. Example ["block@example.com", "@example1.com"]

Root:
  # Create inital user
  EmailAddress: "" # If not supplied, 'instructor@local.com' will be used.
  Password: "" # password for the above email```
````

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
