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

### `\etc\hosts`

```plain
# hosts 檔加上以下內容，local DNS 才有辦法解析 Docker Container
127.0.0.1    mongo-poc-node-1 mongo-poc-node-3 mongo-poc-node-3
```

### Docker

- 執行 Docker Compose

```bash
# 第一次執行
$ docker-compose -f docker-compose.yaml -p mongo-poc  up -d

# 重新編譯相依服務 Image
$ docker-compose -f docker-compose.yaml -p mongo-poc  up -d --build

# MongoDB Replica Set 服務啟動後，需要執行以下指令初始化 replica set
$ docker exec mongo-poc-node-1 bash -c "mongosh mongodb://mongo-poc-node-1:30001 < /data/init-replica.js"
```

- 驗證 Replica Set

```bash
# 進入 mongo-poc-node-1 並執行 mongosh
$ docker exec -it mongo-poc-node-1 mongosh mongodb://mongo-poc-node-1:30001

# Authorization
$ use admin
$ db.auth('root','wf6254fFED234');

# 驗證 Replica Set
$ rs.status().members.map(m => `${m.name}(${m.stateStr})`).join('\n')
mongo-poc-node-1:30001(PRIMARY)
mongo-poc-node-2:30002(SECONDARY)
mongo-poc-node-3:30003(SECONDARY)
```

- 在 Container 中連線 Replica Set

```bash
$ docker exec -it mongo-poc-node-1 bash
$ mongosh mongodb://mongo-poc-node-1:30001,mongo-poc-node-2:30002,mongo-poc-node-1:30003/?replicaSet=mrs
```

### DB - concurrency

- 使用 GUI 連線
    - Connection String: `mongodb://mongo-poc-node-1:30001,mongo-poc-node-2:30002,mongo-poc-node-3:30003/?replicaSet=mrs`
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