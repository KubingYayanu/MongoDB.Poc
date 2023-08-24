use admin;
const username = process.env.MONGO_INITDB_ROOT_USERNAME;
const password = process.env.MONGO_INITDB_ROOT_PASSWORD;
db.auth(username,password);

var config = {
  _id: "MRS",
  version: 1,
  members: [
    { _id: 0, host: "mongo1:27017", priority: 1 },
    { _id: 1, host: "mongo2:27017", priority: 0.5 },
    { _id: 2, host: "mongo3:27017", priority: 0.5 }
  ]
};
rs.initiate(config, { force: true });