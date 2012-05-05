
    PRAGMA foreign_keys = OFF

    drop table if exists "Customer"

    PRAGMA foreign_keys = ON

    create table "Customer" (
        Id  integer primary key autoincrement,
       Code TEXT,
       Name TEXT
    )
