# 🔐 FileCrypto WPF

A comprehensive desktop utility designed for file security and data integrity verification. This application allows users to encrypt and decrypt sensitive files and calculate cryptographic hashes (MD5, SHA-1, SHA-256) to ensure file integrity.

## ✨ Features

* **File Encryption & Decryption:** Secure your files using standard algorithms (AES).
* **Hash Calculation:** Compute file hashes to verify integrity or compare files.
    * Supported Algorithms: MD5, SHA-1, SHA-256, SHA-512.
* **Responsive UI:** Fully asynchronous operations ensure the interface remains responsive even during large file processing.
* **Progress Tracking:** Real-time progress bar for long-running operations.
* **Modern Layout:** Clean and organized user interface built with WPF Grids and Styles.

## 🛠 Technologies & Architecture

The project is built with **.NET / C#** and follows clean coding principles:

* **WPF (Windows Presentation Foundation):** For the UI layer.
* **Asynchronous Programming (Async/Await):** Used extensively to prevent UI freezing during heavy I/O tasks.
* **Layered Architecture:**
    * `Project.Core`: Contains the business logic, interfaces, and enums (e.g., `IHashService`, `HashType`).
    * `Project.Wpf`: Handles the presentation logic and user interaction.
* **IProgress&lt;T&gt; Pattern:** Implemented to report operation progress from the service layer to the UI thread safely.

## 🚀 Getting Started

1.  Clone the repository.
2.  Open the solution in **Visual Studio**.
3.  Build and run the project.
4.  Select a file, choose an operation (Encrypt/Decrypt or Hash), and view the results.

## 📸 Screenshots

<img width="793" height="647" alt="Screenshot 2026-01-11 202957" src="https://github.com/user-attachments/assets/c0858e09-52fa-450c-94f8-3fbe7a77047c" />

## 📄 License

This project is licensed under the MIT License.# FileEncryptorApp
