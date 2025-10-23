# 🚀 Royal Mail Mars Logistics Division

## .NET Coding Exercise

You wake up one morning to discover that Elon Musk and a selection of brave souls have successfully colonised Mars! Royal Mail has partnered with SpaceX to deliver parcels to the Martian colony via rockets launching from **Starport Thames Estuary**.

You're invited to complete the challenge that shows off your **.NET and API skills**.

---

## 📦 Parcel Tracking API (Mars Edition)

Royal Mail now delivers to the single Martian city: **New London**, a geodesic colony near the base of Olympus Mons. Every delivery lands at a central hub and is handed off to AI-powered drones that complete final delivery within 1 hour.

Your backend API must:
- Register parcels
- Track delivery progress
- Support state transitions

> Note: This is an MVP. It’s not expected to be feature-complete.

### 💡 Optional Enhancements
You may propose design changes or additional features, such as:
- Handling delays
- Simulating lost parcels randomly
- Filtering on the `GET /parcels` endpoint

Document any enhancements in your README.

---

## 🔧 REST API Requirements

| Method | Endpoint               | Description                                 |
|--------|------------------------|---------------------------------------------|
| POST   | `/parcels`             | Register a new parcel                       |
| PATCH  | `/parcels/{barcode}`   | Status updates (automated system)          |
| GET    | `/parcels/{barcode}`   | Retrieve status + delivery history         |

---

## 1️⃣ Register a Parcel

**POST** `/parcels`

### Request Example
`{
  "barcode": "RMARS1234567890123456789M",
  "sender": "Anders Hejlsberg",
  "recipient": "Elon Musk",
  "deliveryService": "Express",
  "contents": "Signed C# language specification and a birthday card"
}`

### Behavior
On creation, a parcel record should:
- Assign initial status: `"Created"`
- Set origin: `"Starport Thames Estuary"`
- Set destination: `"New London"`
- Compute and store:
  - `launchDate` (based on delivery service)
  - `etaDays`
  - `estimatedArrivalDate`

> Use Earth UTC time throughout

### Delivery Services

| Service                  | Launch Schedule                  | ETA     |
|--------------------------|----------------------------------|---------|
| Standard Orbital Delivery| Every 26 months (next: 2025-10-01)| 180 days|
| Express Launch Service   | First Wednesday of each month    | 90 days |

### Response Example
`
{
  "barcode": "RMARS1234567890123456789M",
  "status": "Created",
  "launchDate": "2025-09-03",
  "etaDays": 90,
  "estimatedArrivalDate": "2025-12-02",
  "origin": "Starport Thames Estuary",
  "destination": "New London",
  "sender": "Anders Hejlsberg",
  "recipient": "Elon Musk",
  "contents": "Signed C# language specification and a Christmas card"
}
`

---

## 2️⃣ Update Parcel Status

**PATCH** `/parcels/{barcode}`

### Request Example
`
{
  "newStatus": "OnRocketToMars"
}
`

### Valid Statuses
- Created
- OnRocketToMars
- LandedOnMars
- OutForMartianDelivery
- Delivered
- Lost

🚫 No return policy due to delivery costs

### Valid Transitions

| Current Status        | Allowed Transitions           |
|-----------------------|-------------------------------|
| Created               | OnRocketToMars                |
| OnRocketToMars        | LandedOnMars, Lost            |
| LandedOnMars          | OutForMartianDelivery         |
| OutForMartianDelivery | Delivered, Lost               |

> Terminal statuses: `Delivered`, `Lost`

### Guidelines
- Invalid transitions → `400 Bad Request` with explanation
- Use system time to validate transitions
- Simulate time progression via DI or mockable clock (document in README)

### Response
- `200 OK` on success
- `400 Bad Request` if invalid

---

## 3️⃣ Retrieve Parcel

**GET** `/parcels/{barcode}`

### Response Example
`
{
  "barcode": "RMARS1234567890123456789M",
  "status": "Delivered",
  "launchDate": "2025-09-03",
  "estimatedArrivalDate": "2025-12-02",
  "origin": "Starport Thames Estuary",
  "destination": "New London",
  "sender": "Anders Hejlsberg",
  "recipient": "Elon Musk",
  "contents": "Signed C# language specification and a Christmas card",
  "history": [
    { "status": "Created", "timestamp": "2025-08-20" },
    { "status": "OnRocketToMars", "timestamp": "2025-09-03" },
    { "status": "LandedOnMars", "timestamp": "2025-12-02" },
    { "status": "OutForMartianDelivery", "timestamp": "2025-12-02" },
    { "status": "Delivered", "timestamp": "2025-12-02" }
  ]
}
`

---

## 🧪 Barcode Format

- Must match: `RMARS` + 19 digits + 1 uppercase letter
- Example: `RMARS1234567890123456789M`
- Invalid barcodes → `400 Bad Request`

---

## ✅ Sample Happy Path

### Step 1 – Create Parcel
`
{
  "barcode": "RMARS1234567890123456789M",
  "sender": "Anders Hejlsberg",
  "recipient": "Elon Musk",
  "deliveryService": "Express",
  "contents": "Signed C# language specification and a Christmas card"
}
`

### Step 2 – Update to OnRocketToMars
`
{ "newStatus": "OnRocketToMars" }
`

### Step 3 – Update to LandedOnMars
`
{ "newStatus": "LandedOnMars" }
`

### Step 4 – Update to OutForMartianDelivery
`
{ "newStatus": "OutForMartianDelivery" }
`

### Step 5 – Final Delivery
`
{ "newStatus": "Delivered" }
`

### Step 6 – Retrieve Parcel
Call `GET /parcels/{barcode}` to view full history.

---

## 📁 How to Submit

- Send the code as a PR on this repo or zipped folder
- Use .NET 8 or later ONLY
- In-memory data or JSON file (no DB)
- The code must compile and all dependencies must be via Nuget packages
- README with:
  - Setup instructions (`dotnet run`, `dotnet test`)
  - Design choices and trade-offs
  - Walkthrough and scalability thoughts
  - Assumptions or shortcuts
  - AI tool usage

### If you have a Github account ###

Fork this repo, place your code in the /src folder, and submit a PR when you're done, adding your comments to the PR as the README. 
Don't place your comments in the repo README in the root - please leave this intact.

---

## 🧪 Evaluation Criteria

| Area           | What We Look For                                |
|----------------|--------------------------------------------------|
| API Design     | Clear routing, separation of concerns            |
| Logic Correctness | Time-based logic, status transitions, barcode validation |
| Testing        | Meaningful automated tests                       |
| Code Quality   | Readable, testable, maintainable code            |
| Communication  | Clear README, good commit hygiene                |
| AI Use         | Stated, justified, verified in README            |

---

## 🤖 AI Usage Guidelines

✅ Allowed:
- Docs and setup instructions
- Automated tests
- Test data

🚫 Not Allowed:
- Core application logic (status transitions, date/time handling, validation)

Include in README:
- Tools used
- Purpose
- How correctness was verified

---

## ⏱ Time Expectation

Target: **2–3 hours**

- Focus on code quality over completeness. Document trade-offs.  
- Have fun — the future of Martian deliveries may depend on you!
