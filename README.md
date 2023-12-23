# Clinic Management System

## Overview

The Clinic Management System is a web application designed to streamline the management of appointments, doctor schedules, and patient information in a medical clinic. It provides a user-friendly interface for both clinic staff and patients, enhancing the overall efficiency of clinic operations.

## Features

1. **User Roles:**

   - **Admin:** Manages doctors, schedules, and user accounts.
   - **Doctor:** Views and manages their schedules.
   - **Patient:** Books appointments and views their medical history.

2. **Appointment Management:**

   - Patients can schedule appointments with specific doctors.
   - Doctors can view and manage their appointment schedules.

3. **Doctor Schedules:**

   - Admin can create and manage doctor schedules.
   - Doctors can view their own schedules.

4. **User Authentication:**
   - Secure user authentication for admin, doctors, and patients.

## Getting Started

### Prerequisites

- [ASP.NET 8 Core](https://dotnet.microsoft.com/download)
  Tutorial: [Get started with ASP.NET Core MVC](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0&tabs=visual-studio-code)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/gch144/clinic-management-system.git
   ```

2. Navigate to the project directory:

   ```bash
   cd clinic-management-system
   ```

3. Restore dependencies:

   ```bash
   dotnet restore
   ```

4. Update database:

   ```bash
   dotnet ef database update
   ```

5. Run the application:

   ```bash
   dotnet run
   ```

   The application will be accessible at `https://localhost:5003` by default.

## Usage

1. Access the application in your web browser.
2. Log in with your user credentials.
3. Explore different features based on your role (Admin, Doctor, or Patient).
4. Enjoy the streamlined clinic management experience!

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or create a pull request.

## License

This project is licensed under the [MIT License](LICENSE).

---

Feel free to enhance and customize this README based on the specific functionalities and details of your clinic management system.
