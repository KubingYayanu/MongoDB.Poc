use admin;
const username = process.env.MONGO_INITDB_ROOT_USERNAME;
const password = process.env.MONGO_INITDB_ROOT_PASSWORD;
db.auth(username,password);

var config = {
  _id: "mrs",
  version: 1,
  members: [
    { _id: 0, host: "mongo-poc-node-1:30001", priority: 3 },
    { _id: 1, host: "mongo-poc-node-2:30002", priority: 2 },
    { _id: 2, host: "mongo-poc-node-3:30003", priority: 1 }
  ]
};
rs.initiate(config, { force: true });