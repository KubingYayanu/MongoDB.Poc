# MongoDB.Poc

驗證 MongoDB 對於併發更新/新增的特性

## Standalone

### Docker

```bash
$ docker pull mongo:6.0.6
$ docker volume create volume-mongodb
$ docker run --restart=always --name mongodb -v volume-mongodb:/data/db -p 2000:27017 -d mongo:6.0.6
```

### DB - gifts

Connection String: `mongodb://localhost:2000`

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

## Replica Set

### Docker

- 初始化 Containers

```bash
$ docker network create mongo-cluster
$ docker volume create volume-mongo1
$ docker volume create volume-mongo2
$ docker volume create volume-mongo3
$ docker run --restart=always --name mongo1 -v volume-mongo1:/data/db --net mongo-cluster -p 3001:27017 -d mongo:6.0.6 mongod --replSet MRS
$ docker run --restart=always --name mongo2 -v volume-mongo2:/data/db --net mongo-cluster -p 3002:27017 -d mongo:6.0.6 mongod --replSet MRS
$ docker run --restart=always --name mongo3 -v volume-mongo3:/data/db --net mongo-cluster -p 3003:27017 -d mongo:6.0.6 mongod --replSet MRS
```

- 設定 Replica Set

```bash
# 進入 mongo1 並執行 mongosh
$ docker exec -it mongo1 mongosh

# 進入 mongosh 模式，執行下列語法
$ config = {
	"_id" : "MRS",
	"members" : [
		{
			"_id" : 0,
  			"host" : "mongo1:27017"
  		},
  		{
  			"_id" : 1,
  			"host" : "mongo2:27017"
  		},
  		{
  			"_id" : 2,
  			"host" : "mongo3:27017"
  		}
  	]
};

# 使用 config 初始化 Replica Set
$ rs.initiate(config);

# 驗證 Replica Set
$ rs.status().members.map(m => `${m.name}(${m.stateStr})`).join('\n')
```

- 在 Container 中連線 Replica Set

```bash
$ docker exec -it mongo2 bash
$ mongosh mongodb://mongo1:27017,mongo2:27017,mongo3:27017/?replicaSet=MRS
```

### DB - concurrency

- 使用 GUI 連線
    - Connection String: `mongodb://localhost:3001`
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