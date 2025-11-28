# Neo RPG API

A backend proof-of-concept for managing RPG characters and simulating battles.  
Built in **.NET 8**, following clean architecture principles, with full support for character creation, listing, details, and battle execution.

---

## Features

### **1. Character Creation**
Create a character by providing:
- **Name** (4–15 chars, letters or underscore)
- **Job** (`Warrior`, `Thief`, `Mage`)

Each job comes with predefined base stats and modifiers.

### **2. Character Listing**
Retrieve characters with pagination.  
Response includes:
- Id  
- Name  
- Job  
- IsAlive  

### **3. Character Details**
Retrieve full information:
- Base stats  
- Current HP / Max HP  
- Attack modifier  
- Speed modifier  

### **4. Battle System**
Simulates a turn-based battle between two characters:
- Turn order is determined by random speed values derived from job modifiers  
- Attacks use random values based on attack modifiers  
- Battle ends when one character reaches **0 HP**  
- A full battle log is returned  

---

##  Running the Project

Ensure you have:
- **.NET 8 SDK**
- Any REST client (Insomnia, Postman, ThunderClient)

### **Run the API**
```bash
dotnet restore
dotnet build
dotnet run
```

By default the API runs on:

```
https://localhost:7200
http://localhost:5200
```

---

## API Endpoints

### Create Character
**POST** `/api/characters`

#### Request Body
```json
{
  "name": "Arthas",
  "job": "Warrior"
}
```

---

### ** List Characters**  
**GET** `/api/characters?page=1&pageSize=10`

#### Response
```json
[
  {
    "id": "d0fa8c9a-7e66-4fa1-9c95-891ce0ad3c57",
    "name": "Arthas",
    "job": "Warrior",
    "isAlive": true
  }
]
```

---

### Character Details
**GET** `/api/characters/{id}`

#### Response
```json
{
  "id": "d0fa8c9a-7e66-4fa1-9c95-891ce0ad3c57",
  "name": "Arthas",
  "job": "Warrior",
  "maxHp": 20,
  "currentHp": 20,
  "stats": {
    "strength": 10,
    "dexterity": 5,
    "intelligence": 5
  },
  "modifiers": {
    "attack": 10,
    "speed": 4
  }
}
```

---

### Execute Battle 
**POST** `/api/battle`

#### Request Body
```json
{
  "firstPlayer": "GUID_OF_PLAYER_1",
  "secondPlayer": "GUID_OF_PLAYER_2"
}
```

#### Response
```json
{
  "winner": {
    "id": "player1-guid",
    "name": "Arthas",
    "job": "Warrior"
  },
  "loser": {
    "id": "player2-guid",
    "name": "Valeera",
    "job": "Thief"
  },
  "battleLog": "Battle between Arthas (Warrior) - 20 HP and Valeera (Thief) - 15 HP begins! ..."
}
```

---

## Unit Testing

This project uses:

- **xUnit** for unit tests  
- **Moq** for mocking dependencies  
- **FluentValidation.TestHelper** for validator tests  

Run the tests:

```bash
dotnet test
```

---

## Project Structure

```
Neo.RPG/
 ├── Controllers/
 ├── Domain/
 ├── Application/
 ├── Infrastructure/
 ├── Validators/
 ├── Tests/
 └── README.md
```

---

## Technologies

- **.NET 8**
- **Minimal API / Controllers**
- **FluentValidation**
- **Dependency Injection**
- **xUnit + Moq**
