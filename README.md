# 🧠 Scrum Trainer

Web application to train and prepare for the Scrum.org PSM I certification exam.

---

## 🔍 Overview

**Scrum Trainer** is a Blazor-based training platform designed to simulate the experience of the Professional Scrum Master I (PSM I) certification exam by Scrum.org.  
The system provides a timed quiz with single and multiple choice questions, navigation controls, and final scoring — replicating the exam format to support high-quality practice.

---

## 🎯 Purpose of the Project

This project was created with two primary goals:

1. **Help users prepare for the PSM I exam** through realistic, repeatable practice sessions.
2. **Serve as part of my software engineering portfolio**, demonstrating:
   - Clean Code and maintainable structure
   - Dependency Inversion as core architectural principle
   - Unit Test orientation for long-term reliability
   - Front-end UI development with Blazor
   - Planning for authentication, data persistence and user-based progress analysis

The repository will continue to evolve with more features over time.

---

## ✨ Main Features

| Feature | Status |
|--------|:------:|
| Timed exam-like quiz | ✅ |
| Single/multiple answer questions | ✅ |
| Navigate forward/back between questions | ✅ |
| End quiz manually & review results | ✅ |
| Correct/incorrect answer feedback | ✅ |
| Clean Code + Unit Testing | 🧪 active |
| Responsive UI | 🧪 improving |
| User accounts & login system | 🔜 planned |
| Result history stored on user profile | 🔜 planned |
| Analytics and progress evolution | 🟦 future proposal |

---

## 🛠 Tech Stack

| Layer | Technology |
|------|------------|
| UI + App Logic | **Blazor** (WASM/Server depending on build) |
| Web Framework | **ASP.NET Core (.NET 8)** |
| Database | Planned (Entity Framework Core) |
| Authentication | Planned — OAuth2 / Identity concepts |
| Architecture | Clean Code + DIP + best practices |

---

## 🔧 Installation & Local Execution

### 1️⃣ Clone the repository

```bash
git clone https://github.com/dpm76/Scrum-Trainer.git
cd Scrum-Trainer
````

---

### 2️⃣ Run locally (development mode)

Ensure **.NET 8 SDK** is installed.

```bash
dotnet run
```

Visit the app in your browser:

```text
http://localhost:5264
```

or with HTTPS

```text
https://localhost:7055
```

Hot reload will apply code changes instantly.

---

### 3️⃣ Build for production

```bash
dotnet publish --configuration Release
```

The deployable bundle will be generated in:

```bash
/bin/Release/net8.0/publish/
```

Can be hosted in any ASP.NET serving environment.

---

## 🚀 Roadmap

### 📌 Short-term roadmap

- Authentication & user login (planned)
- Database integration for saving results
- User progress history view

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
