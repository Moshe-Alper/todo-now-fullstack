# Todo Now Fullstack

A full-stack todo application built with Angular (frontend) and ASP.NET Core (backend), deployed to Azure App Service.

## 🌐 Live Application

**Frontend:** https://todo-now-gveva8cbaqffe9gr.canadacentral-01.azurewebsites.net/

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 18.x or higher** - [Download](https://nodejs.org/)
- **npm** (comes with Node.js)
- **Azure Cosmos DB account** (for database)

## 🚀 Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd todo-now-fullstack
```

### 2. Backend Setup

```bash
cd backend
dotnet restore src/TodoApi.csproj
```

### 3. Frontend Setup

```bash
cd frontend
npm install
```

## 🏃 Running Locally

### Running the Backend

1. Navigate to the backend directory:
   ```bash
   cd backend
   ```

2. Configure Cosmos DB connection string using user secrets:
   ```bash
   dotnet user-secrets set "CosmosDb:ConnectionString" "AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=your-key-here;" --project src/TodoApi.csproj
   ```
   
   Or set it in `appsettings.json` (located at `backend/appsettings.json`):
   ```json
   {
     "CosmosDb": {
       "ConnectionString": "AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=your-key-here;"
     }
   }
   ```

3. Run the backend:
   ```bash
   dotnet run --project src/TodoApi.csproj
   ```

   The API will be available at: `http://localhost:5000`

### Running the Frontend

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Start the development server:
   ```bash
   npm start
   ```
   
   Or using Angular CLI:
   ```bash
   ng serve
   ```

3. Open your browser and navigate to: `http://localhost:4200`

   The frontend will automatically connect to the backend API at `http://localhost:5000`

## ⚙️ Environment Variables Configuration

### Backend Environment Variables

The backend uses the following configuration:

#### Development (User Secrets)
```bash
dotnet user-secrets set "CosmosDb:ConnectionString" "AccountEndpoint=...;AccountKey=...;"
```

#### Production (Azure App Settings)
In Azure Portal → App Service → Environment variables → App settings → Add:
- **Name:** `CosmosDb__ConnectionString` (use double underscore `__`)
- **Value:** Your Cosmos DB connection string

### Frontend Environment Variables

The frontend uses environment files located in `frontend/src/environments/`:

#### Development (`environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
}
```

#### Production (`environment.prod.ts`)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://todo-now-gveva8cbaqffe9gr.canadacentral-01.azurewebsites.net'
}
```

The production build automatically uses `environment.prod.ts` when building with:
```bash
ng build --configuration production
```

## 📁 Project Structure

```
todo-now-fullstack/
├── backend/
│   ├── src/
│   │   ├── Controllers/     # API controllers
│   │   ├── Models/          # Data models
│   │   ├── Services/        # Business logic
│   │   └── Program.cs       # Application entry point
│   └── appsettings.json     # Configuration
├── frontend/
│   ├── src/
│   │   ├── app/            # Angular application
│   │   ├── assets/          # Static assets
│   │   └── environments/   # Environment configurations
│   └── angular.json         # Angular configuration
└── .github/
    └── workflows/           # CI/CD workflows
```

## 🛠️ Technology Stack

- **Frontend:** Angular 19, TypeScript, SCSS
- **Backend:** ASP.NET Core 8.0, C#
- **Database:** Azure Cosmos DB
- **Deployment:** Azure App Service
- **CI/CD:** GitHub Actions

## 📝 API Endpoints

- `GET /todos` - Get all todos
- `GET /todos/{id}` - Get a specific todo
- `POST /todos` - Create a new todo
- `PUT /todos/{id}` - Update a todo
- `DELETE /todos/{id}` - Delete a todo

All endpoints require the `X-User-Id` header (defaults to "demo" if not provided).

## 🚀 Deployment

The application is automatically deployed to Azure via GitHub Actions when changes are pushed to the `main` branch.

The deployment process:
1. Builds the Angular frontend
2. Copies frontend files to backend `wwwroot`
3. Builds and publishes the .NET backend
4. Deploys to Azure App Service
