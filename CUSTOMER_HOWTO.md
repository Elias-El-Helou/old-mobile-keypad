# OldPhonePad Decoder - Customer Integration Guide

Welcome! This guide shows you **how to use the OldPhonePad Decoder API** in your application.

---

## Table of Contents

1. [What is This?](#what-is-this)
2. [Getting Started](#getting-started)
3. [Using the API](#using-the-api)
4. [Examples](#examples)
5. [Troubleshooting](#troubleshooting)

---

## What is This?

The OldPhonePad Decoder API converts old mobile phone keypad sequences (T9 input) into readable text.

**Example:**

```
You press: 4, 4, 3, 3, 5, 5, 5, space, 5, 5, 5, 6, 6, 6, send
API converts to: "HELLO"
```

### When Would You Use This?

- Converting legacy T9 input data
- Building retro phone simulators
- Decoding historical text messages
- Educational projects about input methods

---

## Getting Started

### Prerequisites

- Internet connection to call the API
- An HTTP client (browser, curl, Postman, etc.)
- Knowledge of JSON format (basic)

### Starting the Service

The API runs locally on your machine. Start it with:

```bash
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project OldMobileKeypad.Api/OldMobileKeypad.Api.csproj
```

You should see:

```
Now listening on: http://localhost:5000
```

The API is now ready to accept requests.

---

## Using the API

### 1. Health Check (Test if API is Running)

**Request:**

```bash
curl http://localhost:5000/api/oldphonepad/health
```

**Response:**

```json
{
  "status": "healthy",
  "service": "OldPhonePad Decoder API",
  "version": "1.0.0",
  "timestamp": "2026-08-20T10:30:45.123Z"
}
```

**What this means:** The API is running and ready.

---

### 2. Get Keypad Mapping (Reference)

**Request:**

```bash
curl http://localhost:5000/api/oldphonepad/keypad-map
```

**Response:**

```json
{
  "description": "Old Phone Keypad Button Mapping",
  "buttons": {
    "0": " ",
    "1": "&'(",
    "2": "abc",
    "3": "def",
    "4": "ghi",
    "5": "jkl",
    "6": "mno",
    "7": "pqrs",
    "8": "tuv",
    "9": "wxyz"
  },
  "special_keys": {
    "backspace": "*",
    "send": "#",
    "pause": "space (space character in input)"
  }
}
```

**What this means:** This shows you which buttons have which letters.

---

### 3. Decode a Keypad Sequence (Main Feature)

This is the core functionality. Send a keypad sequence and get the decoded text.

**Request Format:**

```bash
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"YOUR_SEQUENCE_HERE#"}'
```

**Important:** Your input must always end with `#` (the "send" button).

---

## Examples

### Example 1: Simple Word "HELLO"

**Step 1:** Figure out the sequence

```
H = button 4, pressed twice (44)
E = button 3, pressed twice (33)
L = button 5, pressed three times (555)
L = button 5, pressed three times (555) — need pause after first L
O = button 6, pressed three times (666)
Send = #
```

**Step 2:** Combine with pauses between same-button sequences

```
4433555 555666#
```

**Step 3:** Send to API

```bash
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"4433555 555666#"}'
```

**Response:**

```json
{
  "result": "HELLO",
  "success": true,
  "error": null
}
```

**Success!** The API returned "HELLO".

---

### Example 2: Simple Single Letter "E"

Button 3 has "def", so pressing 3 twice gives "E".

```bash
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"33#"}'
```

**Response:**

```json
{
  "result": "E",
  "success": true,
  "error": null
}
```

---

### Example 3: Using Backspace

What if you make a mistake? Use `*` to delete the last character.

```
2 (A) + 2 (cycle to B) + * (backspace) = A
```

**Sequence:** `227*#`

```bash
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"227*#"}'
```

**Response:**

```json
{
  "result": "B",
  "success": true,
  "error": null
}
```

---

Quick Start with the Demo App

The easiest way to test the API is using the included demo app.

Step 1: Start the API

bash
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project OldMobileKeypad.Api/

Step 2: Open the demo

Demo/index.html

Double-click the file or cd Demo and run `python -m http.server 8000` in your terminal to open it in your browser `localhost:8000`.

Step 3: Test it

The demo shows "🟢 Connected" when the API is running
Enter a T9 sequence and click "Decode"
Try the quick example buttons

---

## Using the Interactive API Documentation

You can also test the API visually without curl.

**Step 1:** Open your browser to:

```
http://localhost:5000
```

You'll see the Swagger UI - an interactive dashboard.

**Step 2:** Find "POST /api/oldphonepad/decode"

**Step 3:** Click "Try it out"

**Step 4:** In the request body, enter:

```json
{
  "input": "4433555 555666#"
}
```

**Step 5:** Click "Execute"

The response will appear below.

---

## Error Handling

The API tells you if something is wrong.

### Error Example 1: Missing `#`

**Request:**

```bash
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"4433555"}'
```

**Response:**

```json
{
  "result": null,
  "success": false,
  "error": "Input must end with '#' (send command)."
}
```

**Fix:** Add `#` to the end: `"4433555#"`

---

### Error Example 2: Empty Input

**Request:**

```bash
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":""}'
```

**Response:**

```json
{
  "result": null,
  "success": false,
  "error": "Input sequence cannot be empty."
}
```

**Fix:** Provide at least one button press: `"2#"`

---

## Code Examples

### JavaScript (Fetch API)

```javascript
async function decodeKeypad(input) {
  const response = await fetch("http://localhost:5000/api/oldphonepad/decode", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ input }),
  });

  const data = await response.json();

  if (data.success) {
    console.log("Decoded:", data.result);
  } else {
    console.error("Error:", data.error);
  }
}

// Usage
decodeKeypad("4433555 555666#"); // Output: "HELLO"
```

### Python (Requests)

```python
import requests
import json

def decode_keypad(input_sequence):
    url = "http://localhost:5000/api/oldphonepad/decode"
    payload = {"input": input_sequence}
    headers = {"Content-Type": "application/json"}

    response = requests.post(url, json=payload, headers=headers)
    data = response.json()

    if data["success"]:
        print(f"Decoded: {data['result']}")
    else:
        print(f"Error: {data['error']}")

# Usage
decode_keypad("4433555 555666#")  # Output: "HELLO"
```

---

## Troubleshooting

### Issue: "Connection refused" or "Unable to connect"

**Solution 1:** Make sure the API is running:

```bash
dotnet run --project OldMobileKeypad.Api/OldMobileKeypad.Api.csproj
```

**Solution 2:** Make sure you are in the Development environment:

```bash
$env:ASPNETCORE_ENVIRONMENT="Development"
```

Check that you see `Now listening on: http://localhost:5000`

---

### Issue: "Input must end with '#'"

**Solution:** All sequences must end with `#`:

```bash
# Wrong
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"4433555 555666"}'

# Correct
curl -X POST http://localhost:5000/api/oldphonepad/decode \
  -H "Content-Type: application/json" \
  -d '{"input":"4433555 555666#"}'
```

Happy decoding!
