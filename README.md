# MongoDB.Poc

驗證 MongoDB 對於併發更新/新增的特性

## Docker

```bash
$ docker pull mongo:6.0.6
$ docker volume create volume-mongodb
$ docker run --restart=always --name mongodb -v volume-mongodb:/data/db -p 2000:27017 -d mongo:6.0.6
```

## MongoDB

Connection String: `mongodb://localhost:2000`

### DB gifts

- Initial Collection/Documents

```text
// mongosh
use gift;
db.records.insertMany([
  {
    _id: ObjectId("649031f3fa4567a2487ffb95"),
    point: 0
  },
  {
    _id: ObjectId("649057d2a4a33c0393e658f0"),
    point: 0
  },
  {
    _id: ObjectId("6490fd82d66481504b125ba6"),
    point: 0
  },
  {
    _id: ObjectId("649103e8d66481504b125ba7"),
    point: 0
  },
  {
    _id: ObjectId("649a946a677a2ecd7f9273bf"),
    point: 0
  }
]);
```

### DB concurrency

- Initial Collection/Documents

```text
// mongosh
use concurrency;
db.member.insertOne({
  _id: ObjectId("649d39e57a838f1a21139aef"),
  point: {
    balance: 0
  }
});

db.createCollection("point_record");
db.createCollection("point_expiration");
```