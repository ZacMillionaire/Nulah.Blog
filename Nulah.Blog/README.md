# TODO: Proper Readme file

For now this project is possible to build with an appsettings.json file with a structure as follows:

` 
{
  "MSSQL": {
    "ConnectionString": ""
  },
  "RoleGroups": { // Both values are Guids stored in the database
    "Administrator": "",
    "User": ""
  }
}
`

However you won't be able to do much without the database schema, which I am still working on.

If for some reason you're super keen to give it a shot, let me know and I'll make exposing the schema a priority.