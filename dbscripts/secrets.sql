BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Users" (
       "Id"  TEXT NOT NULL UNIQUE,
       "Username"	   TEXT NOT NULL UNIQUE, 
       "PasswordHash"	   TEXT,
       "PasswordSalt"	   TEXT,
       PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Secrets" (
       "Id"	TEXT NOT NULL UNIQUE,
       "UserId" TEXT NOT NULL,
       "Content"	TEXT,
       "DateCreated"	TEXT NOT NULL,
       "DateModified"	TEXT NOT NULL,
       PRIMARY KEY("Id"),
       FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);
COMMIT;




