# Cosmos Explorer App

A Windows Forms app to explore and manage Azure Cosmos DB databases, containers, and documents.

## Features

- **Load Cosmos DB Keys**

  - Enter your Endpoint URI and Primary Key
  - Click **Load Keys** to connect.Displays connection speed (latency) after loading.
- **Database Management**

  - View all databases in the **Databases** tab.
  - Create new databases by entering a name and clicking **Create Database**.
  - Refresh database list.
  - Count total databases using the **DB Count** button next to the list.
- **Container / Table Management**

  - Switch to the **Containers** tab.
  - Select a database from the dropdown to view its containers (tables).
  - Create new containers by entering a name and clicking **Create Container**.
  - Refresh container list with **Refresh Tables** button.
  - Container creation shows a confirmation message.
- **Document Management**

  - (Future tab for documents, not yet implemented.)

## Screenshots

### Databases Tab

![Databases Tab][dbtab]

### Containers Tab

![Containers Tab][ctab]

## Notes

- Requires **.NET 6+** or later.
- Uses **Microsoft.Azure.Cosmos** SDK.
- Connection speed is measured after loading keys and displayed in milliseconds.

## Usage

1. Clone the repository:

```bash
git clone https://github.com/NatiG7/CosmosExplorerApp.git
```

```bash
dotnet restore
```

```bash
dotnet run
```

[dbtab]: Screenshots/databases_tab.png
[ctab]: Screenshots/tables_tab.png