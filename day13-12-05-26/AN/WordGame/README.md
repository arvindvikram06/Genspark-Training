### How to run:
```bash
dotnet run
```

### Database Schema:
```sql
create Table users(
	id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	name VARCHAR(50),
	email VARCHAR(50) UNIQUE,
	password VARCHAR(50)
);

CREATE TABLE games (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    secret_word VARCHAR(50),
    max_attempts INT,
    total_score INT
);

CREATE TABLE attempts (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    game_id INT REFERENCES games(id) ON DELETE CASCADE,
    guess VARCHAR(50),
    feedback VARCHAR(50),
    attempt_number VARCHAR(10),
    result BOOLEAN,
    attempt_time TIMESTAMP
);


create Table words(
	id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	word VARCHAR(50)
);
```

### Outputs:

![img1](/assets/img1.png)

![img2](/assets/img2.png)

![img3](/assets/img3.png)

![img4](/assets/img4.png)

