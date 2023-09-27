const db = db.getSiblingDB("admin");
const host = process.env.MONGO_HOST;
const username = process.env.MONGO_INITDB_ROOT_USERNAME;
const password = process.env.MONGO_INITDB_ROOT_PASSWORD;
db.auth(username, password);

var config = {
  _id: "mrs",
  version: 1,
  members: [
    {
      _id: 0,
      host: host + ":30001",
      priority: 3
    },
    {
      _id: 1,
      host: host + ":30002",
      priority: 2
    },
    {
      _id: 2,
      host: host + ":30003",
      priority: 1
    }
  ]
};
rs.initiate(config, { force: true });
