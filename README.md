# 🔐 FileCrypto WPF

A comprehensive desktop utility designed for file security and data integrity verification. This application allows users to encrypt/decrypt sensitive files, calculate cryptographic hashes, and **perform image steganography to hide secrets within image files.**

## ✨ Features

* **File Encryption & Decryption:** Secure your files using standard algorithms (AES).
* **Hash Calculation:** Compute file hashes to verify integrity or compare files.
    * Supported Algorithms: MD5, SHA-1, SHA-256, SHA-512.
* **Image Steganography:** Conceal sensitive text or data within image files (e.g., PNG, BMP) using LSB (Least Significant Bit) modification without visually altering the image.
* **Responsive UI:** Fully asynchronous operations ensure the interface remains responsive even during large file processing.
* **Progress Tracking:** Real-time progress bar for long-running operations.
* **Modern Layout:** Clean and organized user interface built with WPF Grids and Styles.

## 🛠 Technologies & Architecture

The project is built with **.NET / C#** and follows clean coding principles:

* **WPF (Windows Presentation Foundation):** For the UI layer.
* **Asynchronous Programming (Async/Await):** Used extensively to prevent UI freezing during heavy I/O tasks.
* **Layered Architecture:**
    * `Project.Core`: Contains the business logic, interfaces, and enums (e.g., `IHashService`, `ISteganographyService`, `HashType`).
    * `Project.Wpf`: Handles the presentation logic and user interaction.
* **IProgress<T> Pattern:** Implemented to report operation progress from the service layer to the UI thread safely.

## 🚀 Getting Started

1.  Clone the repository.
2.  Open the solution in **Visual Studio**.
3.  Build and run the project.
4.  Select a file, choose an operation (**Encrypt/Decrypt, Hash, or Steganography**), and view the results.

## 📸 Screenshots

<img width="1088" height="637" alt="image" src="https://github.com/user-attachments/assets/5baf5a96-aa77-4e06-a3d6-c35a20983e87" />

<img width="1085" height="638" alt="image" src="https://github.com/user-attachments/assets/124e5174-fe43-4e1c-8a38-327fb44eb9a3" />

## 📄 License

This project is licensed under the MIT License.
