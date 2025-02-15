# Stiva Overflow

**Stiva Overflow** is a collaborative discussion platform designed for users to ask questions, share answers, and explore topics across diverse categories.  

You can test the website [here](https://proiectstackoverflow-92560154672.europe-west3.run.app/).
---

## ✨ Key Features  

### 🌐 Homepage  
- Displays **discussion categories** as tags.  
- Each category contains paginated topics (questions).  
- Navigate to dedicated category pages to view all related topics.  

### 🔍 Advanced Search Engine  
- Search topics and answers by keywords (supports partial matches, e.g., "SEPT" for "SEPTEMBER").  

### 🗂️ Sorting & Filtering  
- Both the questions and the answers can be sorted by:  
  - Post date.  
  - Number of replies.  
 

### 👤 User Profile  
- Includes personal details (name, email, bio).  
- Displays recent activity: latest questions and answers posted.  

### 🛠️ Administration  
- **Category Management**: Full CRUD operations.  
- Content Moderation: Remove inappropriate content, adjust topic categories.  
- Manage member access rights.  

---  

# Installation Guide

## 🛠️ Prerequisites
1. **.NET SDK** (version 8.0 or higher)
   - Download: https://dotnet.microsoft.com/download
2. **Database Server** 
   - SQL Server
3. **IDE/Editor**
   - Visual Studio 2022+ **OR**
   - Rider with all needed add-ons
4. **Git** (for cloning the repository)

##  Setup Steps

### 1. Clone the Repository


### 2. Database Configuration
1. Update connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": your-server-string
}
```
2. Apply migrations:
```bash
dotnet ef database update
```
*or use Package Manager Console in Visual Studio:*
```powershell
Update-Database
```

### 3. Restore Dependencies
```bash
dotnet restore
```

### 4. Run the Application
```bash
dotnet run
```
- Access via: `https://localhost:8080` (or port shown in console)
