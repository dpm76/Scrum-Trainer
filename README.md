# 🧠 Scrum Trainer

Web application to train and prepare for the Scrum.org PSM I certification exam.

---

## 🔍 Overview

**Scrum Trainer** is a Blazor-based training platform designed to simulate the experience of the Professional Scrum Master I (PSM I) certification exam by Scrum.org.  
The system provides a timed quiz with single and multiple choice questions, navigation controls, and final scoring — replicating the exam format to support high-quality practice.

---

## 🔗 Live Demo

You can try the application online here:  
👉 **<https://scrum-trainer.davidpm.eu>**

This live version allows you to experience the quiz system, test the timer-based flow, navigate questions, and review results just as you would in a real PSM I practice session.

---

## 🎯 Purpose of the Project

This project was created with two primary goals:

1. **Help users prepare for the PSM I exam** through realistic, repeatable practice sessions.
2. **Serve as part of my software engineering portfolio**, demonstrating:
   - **Clean Code** and maintainable structure
   - **Dependency** Inversion as core architectural principle
   - **Unit Test** orientation for long-term reliability
   - Front-end UI development with **Blazor**
   - Authentication, data persistence and *planned* user-based progress analysis

 👉 The repository will continue to evolve with more features over time.

---

## ✨ Main Features

| Feature | Status |
| ------- | :----: |
| Timed exam-like quiz | ✅ |
| Single/multiple answer questions | ✅ |
| Navigate forward/back between questions | ✅ |
| End quiz manually & review results | ✅ |
| Correct/incorrect answer feedback | ✅ |
| Clean Code + Unit Testing | 🧪 active |
| Responsive UI | 🧪 improving |
| User accounts & login system | ✅ |
| Result history stored on user profile | ✅ |
| Analytics and progress evolution | 🟦 future proposal |

---

## 🛠 Tech Stack

| Layer | Technology |
| ----- | ---------- |
| UI + App Logic | **Blazor** (WASM/Server depending on build) |
| Web Framework | **ASP.NET Core (.NET 8)** |
| Database | Entity Framework Core |
| Authentication | Identity ✅ and OAuth2 (🔜 planned) concepts |
| Architecture | Clean Code + DIP + best practices |

---

## 🎁 Installation & Local Execution

### 💾 Clone the repository

```bash
git clone https://github.com/dpm76/Scrum-Trainer.git
cd Scrum-Trainer
```

### 🔧 Install EntityFramework tools

```bash
dotnet tool install --global dotnet-ef --version 8.0.22
```

### 🗄️ Create and Update Database

```bash
dotnet ef database update --project ScrumTrainer
```

## 📧 Email (SMTP) Configuration

This project uses an SMTP server to send transactional emails (e.g. email confirmation during user registration).

For security reasons, **SMTP credentials are not committed to the repository**. Instead, a local configuration file is used and ignored by Git.

### 🔒 Local configuration file

The repository includes the following template file:

```text
appsettings.Local.json.template
```

To configure email sending locally, follow these steps:

   1. **Create a copy of the template file** and rename it to:

      ```text
      appsettings.Local.json
      ```

   2. **Fill in your SMTP settings** in the newly created file:

      ```json
      {
         "EMailSettings": {
            "SmtpServer": "smtp.mail-server.com",
            "SmtpPort": 587,
            "SenderName": "Scrum-Trainer",
            "SenderEmail": "your-user@your-domain.com",
            "Username": "smtp username",
            "Password": "password/app-key"
      }
      ```

   3. The file `appsettings.Local.json` is listed in `.gitignore`, so it will **never be committed** to the repository.

### ⚙️ How it works

At startup, the application loads configuration in the following order:

   1. `appsettings.json`
   2. `appsettings.Development.json`
   3. `appsettings.Local.json` (if present)

This allows sensitive data (such as SMTP credentials) to remain local while keeping the repository clean and safe.

---

### 📝 Notes

- The SMTP configuration is required for email-based features such as **account confirmation**.
- For production environments, it is recommended to use **environment variables** or a secure secrets manager.

---

### 🚀 Run locally (development mode)

Ensure **.NET 8 SDK** is installed.

```bash
dotnet run --project ScrumTrainer
```

Visit the app in your browser:

```text
http://localhost:5264
```

or with HTTPS

```text
https://localhost:7055
```

### 🧪 Run Tests (optional)

If you do some changes and want to check whether your changes don't break the code.

```bash
dotnet test -v n
```

### 🏗️ Build for production (optional)

```bash
dotnet publish --configuration Release
```

The deployable bundle will be generated in:

```bash
/bin/Release/net8.0/publish/
```

Can be hosted in any ASP.NET serving environment.

---

## 🛣️ Roadmap

### 📌 Short-term roadmap

- Authentication & user login ✅
- Database integration for saving results (🔜 planned)
- User progress history view (🔜 planned)

### 📌 Medium-term roadmap

- Analytics dashboard (graphs, accuracy metrics)
- Question categorization by topic
- Multiple training modes

### 📌 Future improvements

- Community-submitted question pool
- Advanced personalization and score prediction

---

## 📬 Contact

💼 GitHub Portfolio: <https://github.com/dpm76>

📧 Email: [mailto:davidpm.itengineer@gmail.com](davidpm.itengineer@gmail.com)

🔗 LinkedIn: <https://www.linkedin.com/in/dpm-itengineer/>

---

## 📄 License

This project is licensed under the **MIT License**.  

> You are free to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of this software, provided that the original copyright notice and this permission notice are included in all copies or substantial portions of the software.

See the full license in the [LICENSE](LICENSE.txt) file.

---
