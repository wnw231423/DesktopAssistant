# Desktop Assistant
This is a simple desktop assistant that helps you manage your courses and todos.

## To Run
1. Clone the repository
```bash
git clone https://github.com/wnw231423/DesktopAssistant.git
cd DesktopAssistant
```
2. Configure EF framework
- Install the EF Core CLI tools
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- Apply the migrations to create the database
  ```bash
  cd Data
  dotnet ef database update
  ```
3. Run the GUI project in the solution.

## Road Map
1. Basic functionality
   - [x] Course Table
     - [x] Add Course
     - [x] Delete Course
     - [x] Edit Course
     - [x] View Course's Todos
     - [x] Export Courses to Json
   - [x] Todo
     - [x] Add Todo
     - [x] Delete Todo
     - [x] Edit Todo
   - [x] Course Resources
     - [x] Add/Delete Resource type 
     - [x] Add/Delete Resource
     - [x] Open Resource
   - [x] AI Daily Report
2. GUI beautification
   - [x] Color definitions
   - [ ] Custom background
3. Advanced Features