# Sample Inputs and Outputs

This document lists the sample input parameters and corresponding console output screens for the standard workflows of the Library Management System.

---

## 1. Member Registration

### Sample Inputs
* Name: `vikram`
* Email: `vikram@gmail.com`
* Phone Number: `9003847839`

### Expected Console Output
```text
--- Register New Member ---
Enter Name: vikram
Enter Email: vikram@gmail.com
Enter Phone Number: 9003847839

member registered successfully!
```

---

## 2. Searching Books by Title or Author

### Sample Inputs
* Query: `harry`

### Expected Console Output
```text
--- Search Books ---
Enter Title or Author to search: harry
ID: 1 | Title: Harry Potter and the Sorcerer's Stone | Author: J.K. Rowling | Category: Fantasy
```

---

## 3. Searching Books by Category (Dedicated Feature)

### Sample Inputs
* Category Name: `Science Fiction`

### Expected Console Output
```text
--- Search Books by Category ---
Enter Category Name to search: Science Fiction
ID: 2 | Title: Dune | Author: Frank Herbert | Category: Science Fiction
```

---

## 4. Borrowing a Book

### Scenario A: Successful Borrowing
* Book ID: `5`

#### Expected Console Output
```text
--- Borrow a Book ---
Enter Book ID to borrow: 5
Successfully borrowed the book!
```

### Scenario B: Exceeding Membership Tier Borrow Limit
* Member Tier: Basic (Limit: 2 active books)
* Current Borrowings: 2
* Attempted Book ID: `3`

#### Expected Console Output
```text
--- Borrow a Book ---
Enter Book ID to borrow: 3
Failed to borrow: Member has reached the maximum borrow limit.
```

### Scenario C: Blocked by Unpaid Fines Exceeding 500 Rupees
* Current Unpaid Fines: ₹600
* Attempted Book ID: `2`

#### Expected Console Output
```text
--- Borrow a Book ---
Enter Book ID to borrow: 2
Failed to borrow: Member has unpaid fines exceeding ₹500. Please pay fines to continue borrowing.
```

---

## 5. Returning a Checked-out Book

### Sample Inputs
* Book Copy ID: `7`

### Expected Console Output
```text
--- Return a Book ---
Enter Copy ID to return: 7
Successfully returned the book!
```

---

## 6. Fine Payment

### Sample Inputs
* Fine ID: `4`

### Expected Console Output
```text
--- Fine Management ---
1. View Pending Fines & Pay
2. View Fine History
0. Back

Select an option: 1
Fine ID: 4 | Amount: ₹300 | Reason: DAMAGED_BOOK | Status: PENDING

Enter Fine ID to pay: 4
Fine paid successfully!
```
