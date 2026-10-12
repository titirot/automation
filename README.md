[github_readme_file.md](https://github.com/user-attachments/files/33316345/github_readme_file.md)
# Automated Testing Framework for Social Challenges Platform

![Automation Architecture](https://raw.githubusercontent.com/titirot/automation/main/image_e23d58.jpg)

## 📌 Project Overview
This project is an **End-to-End (E2E) Automation Testing Framework** built to validate the core functionality, user flows, and data interactions of a dynamic social challenges and sharing web platform (`http://localhost:4200/`). 

Designed with scalability, maintainability, and reliability in mind, the framework leverages **C#** and **Selenium WebDriver** structured according to the **Page Object Model (POM)** architectural pattern. It incorporates **Data-Driven Testing (DDT)** to decouple test logic from test data, ensuring robust execution across various environment states.

---

## 🛠️ Key Technologies & Frameworks

- **Programming Language:** C# (.NET)
- **Browser Automation:** Selenium WebDriver
- **Test Runner / Framework:** MSTest / NUnit
- **Driver Management:** `WebDriverManager.NET` (Automated ChromeDriver configuration)
- **Data-Driven Engine:** `ExcelReaderFactory` (Parsing `.xlsx` test data files)
- **Design Pattern:** Page Object Model (POM)
- **Version Control:** Git & GitHub

---

## 🏗️ Architecture & Project Structure

The project follows a clean separation of concerns, dividing application pages, executable tests, and dynamic data sources into distinct modules:

```text
challenges/
├── Data/
│   └── TestData.xlsx               # Excel file supplying external dynamic test sets
├── Pages/
│   ├── BasePage.cs                 # Base class containing wrapper methods for Selenium interactions
│   ├── HomePage.cs                 # Page Object for Main Landing & Challenges feed
│   ├── LoginPage.cs                # Page Object for User Authentication
│   └── UploadChallengePage.cs      # Page Object for File/Media Upload & Content Creation
├── Tests/
│   ├── ChallengeTests.cs           # E2E test suites covering challenge workflows & media upload
│   ├── HomePageTests.cs            # Tests covering navigation, UI elements, and feed items
│   └── LoginTests.cs               # Tests covering valid/invalid login states
└── challenges.sln                  # Visual Studio Solution entry point
```

---

## 🚀 Key Features & Implementation Highlights

### 1. Page Object Model (POM)
* All web elements and page-specific interactions are encapsulated inside their respective `Page` classes (`Pages/`).
* Tests (`Tests/`) remain clean, readable, and focused solely on assertions and business logic flow rather than direct driver calls.

### 2. Data-Driven Testing (DDT)
* Dynamic tests read input parameter sets directly from `Data/TestData.xlsx`.
* Utilizes **relative file path resolution** (`AppDomain.CurrentDomain.BaseDirectory`) to guarantee seamless execution across different machines without hardcoded local environment paths.

### 3. Dynamic Environment Setup & Waits
* Integrated `WebDriverManager` ensures automatic retrieval and instantiation of matching browser drivers.
* Configured implicit and explicit waits (`TimeSpan.FromSeconds(10)`) to ensure stability against asynchronous DOM rendering and Angular dynamic components.

---

## 🧪 Test Execution & Verification

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or higher)
* [Visual Studio 2022](https://visualstudio.microsoft.com/) with .NET Desktop Development workload

### Running Tests Locally

1. **Clone the repository:**
   ```bash
   git clone https://github.com/titirot/automation.git
   ```
2. **Open the Solution:**
   Open `challenges.sln` using Visual Studio.
3. **Execute via Test Explorer:**
   * Open **Test Explorer** (`Test` -> `Test Explorer`).
   * Click **Run All Tests** (`Ctrl + R, A`).

---

## 📧 Contact & Portfolio
Developed as part of a professional Quality Assurance & Automation Engineering portfolio.
* **GitHub Repository:** [https://github.com/titirot/automation](https://github.com/titirot/automation)
