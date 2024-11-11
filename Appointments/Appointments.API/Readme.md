# Appointment Management API

This API is designed to manage appointments, providing endpoints for creating, rescheduling, signing, and removing appointments. It also includes automatic expiration handling for appointments past their scheduled date and time.

## Features
- **Appointment Exploration**: Allows listing and querying appointments using id, email or date.
- **Appointment Assignation**: Allows creation and rescheduling of appointments.
- **Appointment Management**: Allows approval, cancellation and deletion of appointments.
- **Role-based Actions**: Allows to differ from user / manager actions by role assignation.
- **Promotion Request**: Allows an ordinary email/user to request a change of role to Manager.
- **Signing**: Manager action for triggering a change the appointment status to: Approved or Cancelled
- **Automatic Expiration**: Background service marks appointments as expired when they pass their scheduled date and time.
- **Flexible Storage**: Supports switching between an in-memory repository (for development) and SQL Server database (for production).
- **Manager Mode**: A toggle switch in Client's page to automatically switch to Manager's view.

---

## API Actions Table

| Action					| HTTP		| URL endpoint						| Description																	|
|---------------------------|-----------|-----------------------------------|-------------------------------------------------------------------------------|
| Create Appointment		| POST		| /api/appointments/create			| Create a new appointment.														|
| Get All Appointments		| GET		| /api/appointments					| Retrieve a list of all appointments.											|
| Get Appointment By ID		| GET		| /api/appointments/{id}			| Retrieve a specific appointment by ID.										|
| Reschedule Appointment	| PUT		| /api/appointments/reschedule/{id}	| Reschedule an existing appointment.											|
| Sign Appointment			| PUT		| /api/appointments/sign			| Sign the appointment (can be requestor or recipient).							|
| Remove Appointment		| DELETE	| /api/appointments/remove/{id}		| Remove (delete) an appointment.												|
| Expire Appointments		| Backend	| N/A								| Automatically marks appointments as expired when the date/time has passed		|


## Prerequisites
- **.NET 9 SDK**
- **SQL Server** for database storage in production mode
- **Entity Framework Core** for data access and migrations

---

## Setup and Configuration

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/ajmarrugos/Appointments.git
   cd Appointments