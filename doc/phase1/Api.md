{
  "swagger": "2.0",
  "info": {
    "version": "1.0",
    "title": "KrewFindr",
    "description": "TODO: Add Description"
  },
  "host": "groups",
  "basePath": "/",
  "schemes": [
    "http"
  ],
  "consumes": [
    "application/json"
  ],
  "produces": [
    "application/json"
  ],
  "paths": {
    "/create": {
      "post": {
        "description": "Creates a new group in an organization and adds the current user as a member if they are not an instructor. Can be called by organization instructor or any student in organization (if permitted by instructor)",
        "summary": "Group Create",
        "tags": [
          "Group"
        ],
        "operationId": "CreatePost",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "orgId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Parent organization of the group"
          },
          {
            "name": "name",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "The name of the group"
          },
          {
            "name": "desiredSkills",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "A skill profile for the group"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "/create",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "orgId=&name=&desiredSkills="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Group Create",
            "x-testDescription": "Creates a new group in an organization and adds the current user as a member if they are not an instructor. Can be called by organization instructor or any student in organization (if permitted by instructor)"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///groups/{GroupId}": {
      "get": {
        "description": "Returns a single group object. Can be called by any user in an organization.",
        "summary": "Group Get",
        "tags": [
          "Group"
        ],
        "operationId": "GroupsByGroupIdGet",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "GroupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "Id of the group"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "///groups/"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Group Get",
            "x-testDescription": "Returns a single group object. Can be called by any user in an organization."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      },
      "put": {
        "description": "Updates a group. Can be called by any member of the group or organization instructor.",
        "summary": "Group Update",
        "tags": [
          "Group"
        ],
        "operationId": "GroupsByGroupIdPut",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "GroupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "Id of the group"
          },
          {
            "name": "name",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "The name of the group"
          },
          {
            "name": "desiredSkills",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "A skill profile for the group"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "PUT",
              "uri": "///groups/",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "name=&desiredSkills="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Group Update",
            "x-testDescription": "Updates a group. Can be called by any member of the group or organization instructor."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      },
      "delete": {
        "description": "Deletes a group. All members are removed from it. Can only be called by organization instructor.",
        "summary": "Group Delete",
        "tags": [
          "Group"
        ],
        "operationId": "GroupsByGroupIdDelete",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "GroupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "Id of the group"
          },
          {
            "name": "Content-Type",
            "in": "header",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "DELETE",
              "uri": "///groups/",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              }
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Group Delete",
            "x-testDescription": "Deletes a group. All members are removed from it. Can only be called by organization instructor."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///users/{userId}/roles/create": {
      "post": {
        "description": "Creates a new role for this user. Roles other than admin are relative to organizations. Role assignment of Admin can only be added by other admins, instructor by admin or other instructor, and student by instructor.",
        "summary": "Role Create",
        "tags": [
          "Role"
        ],
        "operationId": "UsersRolesCreateByUserIdPost",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "User id for which the role will be made"
          },
          {
            "name": "orgId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of organization for which the role is relevant (must be blank for admin)"
          },
          {
            "name": "role",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "One of: admin, student, instructor"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///users//roles/create",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "orgId=&role="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Role Create",
            "x-testDescription": "Creates a new role for this user. Roles other than admin are relative to organizations. Role assignment of Admin can only be added by other admins, instructor by admin or other instructor, and student by instructor."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///users/{userId}/roles/{roleId}": {
      "delete": {
        "description": "Deletes a role for this user. Role assignment of Admin and instructor can only be deleted by other admins, and student by instructor or admin.",
        "summary": "Role Delete",
        "tags": [
          "Role"
        ],
        "operationId": "UsersRolesByUserIdAndRoleIdDelete",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "User id for which the role will be made"
          },
          {
            "name": "roleId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "The role which will be deleted"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "DELETE",
              "uri": "///users//roles/"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Role Delete",
            "x-testDescription": "Deletes a role for this user. Role assignment of Admin and instructor can only be deleted by other admins, and student by instructor or admin."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///users": {
      "get": {
        "description": "Get users by params in the organization",
        "summary": "User List",
        "tags": [
          "User"
        ],
        "operationId": "UsersGet",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "groupid",
            "in": "query",
            "required": true,
            "type": "string",
            "description": ""
          },
          {
            "name": "role",
            "in": "query",
            "required": true,
            "type": "string",
            "description": ""
          },
          {
            "name": "userid",
            "in": "query",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "///users?groupid=&role=&userid="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "User List",
            "x-testDescription": "Get users by params in the organization"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      },
      "delete": {
        "description": "Remove user. Permission: user",
        "summary": "User Delete",
        "tags": [
          "User"
        ],
        "operationId": "UsersDelete",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "userId",
            "in": "query",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "DELETE",
              "uri": "///users?userId="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "User Delete",
            "x-testDescription": "Remove user. Permission: user"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "//users/": {
      "post": {
        "description": "Create a new user.",
        "summary": "User Create",
        "tags": [
          "User"
        ],
        "operationId": "UsersPost",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "lname",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Last Name for the user"
          },
          {
            "name": "fname",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "First Name for the user"
          },
          {
            "name": "email",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Email for the user"
          },
          {
            "name": "role",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Role for the user [student, instructor, admin]"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "//users/",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "lname=&fname=&email=&role="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "User Create",
            "x-testDescription": "Create a new user."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///users/{userId}": {
      "put": {
        "description": "Create a new user",
        "summary": "User Update",
        "tags": [
          "User"
        ],
        "operationId": "UsersByUserIdPut",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          },
          {
            "name": "lname",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Last Name for the user"
          },
          {
            "name": "fname",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "First Name for the user"
          },
          {
            "name": "email",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Email for the user"
          },
          {
            "name": "role",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Role for the user [student, admin ...]"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "PUT",
              "uri": "///users/",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "lname=&fname=&email=&role="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "User Update",
            "x-testDescription": "Create a new user"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "//organizations/": {
      "get": {
        "description": "Get Organization List",
        "summary": "Organization Get",
        "tags": [
          "Organization"
        ],
        "operationId": "OrganizationsGet",
        "produces": [
          "application/json"
        ],
        "parameters": [],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "//organizations/"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Organization Get",
            "x-testDescription": "Get Organization List"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      },
      "post": {
        "description": "Create a new organization. Role: [admin, instructor]",
        "summary": "Organization Create",
        "tags": [
          "Organization"
        ],
        "operationId": "OrganizationsPost",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "name",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "organization name"
          },
          {
            "name": "groupSizeMax",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "max number of teammates in a group"
          },
          {
            "name": "groupSizeMin",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "min number of teammates in a group"
          },
          {
            "name": "courseCode",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "course code"
          },
          {
            "name": "courseTitle",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Title of the course"
          },
          {
            "name": "courseDescription",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "description of course"
          },
          {
            "name": "allowLeavingGroup",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Can students leave a group after joining"
          },
          {
            "name": "skillDomain",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Skill domain students can choose from"
          },
          {
            "name": "createdBy",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "userID of the creator"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "//organizations/",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "name=&groupSizeMax=&groupSizeMin=&courseCode=&courseTitle=&courseDescription=&allowLeavingGroup=&skillDomain=&createdBy="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Organization Create",
            "x-testDescription": "Create a new organization. Role: [admin, instructor]"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///suggestions/{groupId}/groups": {
      "get": {
        "description": "Get a list of Suggestion for groups",
        "summary": "Get Group Suggestions",
        "tags": [
          "Suggestion"
        ],
        "operationId": "SuggestionsGroupsByGroupIdGet2",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "groupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "///suggestions//groups"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Get Group Suggestions",
            "x-testDescription": "Get a list of Suggestion for groups"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///organizations/{organizationId}": {
      "put": {
        "description": "Update organization\n",
        "summary": "Organization Update",
        "tags": [
          "Organization"
        ],
        "operationId": "OrganizationsByOrganizationIdPut",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "organizationId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          },
          {
            "name": "name",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "organization name"
          },
          {
            "name": "groupSizeMax",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "max number of teammates in a group"
          },
          {
            "name": "groupSizeMin",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "min number of teammates in a group"
          },
          {
            "name": "courseCode",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "course code"
          },
          {
            "name": "courseTitle",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Title of the course"
          },
          {
            "name": "courseDescription",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "description of course"
          },
          {
            "name": "allowLeavingGroup",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Can students leave a group after joining"
          },
          {
            "name": "skillDomain",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Skill domain students can choose from"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "PUT",
              "uri": "///organizations/",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "name=&groupSizeMax=&groupSizeMin=&courseCode=&courseTitle=&courseDescription=&allowLeavingGroup=&skillDomain="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Organization Update",
            "x-testDescription": "Update organization\n"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      },
      "delete": {
        "description": "Delete an organization. Delete can only be done by the creator of the organization or the admin.",
        "summary": "Organization Delete",
        "tags": [
          "Organization"
        ],
        "operationId": "OrganizationsByOrganizationIdDelete",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "organizationId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "DELETE",
              "uri": "///organizations/"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Organization Delete",
            "x-testDescription": "Delete an organization. Delete can only be done by the creator of the organization or the admin."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///organizations/{organizationId}/Groups": {
      "get": {
        "description": "Get a list of groups within organization",
        "summary": "Organization Groups",
        "tags": [
          "Organization"
        ],
        "operationId": "OrganizationsGroupsByOrganizationIdGet",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "organizationId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "///organizations//Groups"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Organization Groups",
            "x-testDescription": "Get a list of groups within organization"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///suggestions/{groupId}/users": {
      "get": {
        "description": "Get a list of Suggestion for users",
        "summary": "Get User Suggestions",
        "tags": [
          "Suggestion"
        ],
        "operationId": "SuggestionsUsersByGroupIdGet",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "groupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "///suggestions//users"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Get User Suggestions",
            "x-testDescription": "Get a list of Suggestion for users"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "//reviews": {
      "put": {
        "description": "Update an exsiting review. Permissions: author",
        "summary": "Review Update",
        "tags": [
          "Review"
        ],
        "operationId": "ReviewsPut",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "peerId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of the user this review is for"
          },
          {
            "name": "rating",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Rating for this user"
          },
          {
            "name": "reviewerId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of the user reviewing"
          },
          {
            "name": "groupId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of the group peer participated in"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "PUT",
              "uri": "//reviews",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "peerId=&rating=&reviewerId=&groupId="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Review Update",
            "x-testDescription": "Update an exsiting review. Permissions: author"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      },
      "post": {
        "description": "Create a new review for a group member",
        "summary": "Review Create",
        "tags": [
          "Review"
        ],
        "operationId": "ReviewsPost",
        "produces": [
          "application/json"
        ],
        "consumes": [
          "application/x-www-form-urlencoded"
        ],
        "parameters": [
          {
            "name": "peerId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of the user this review is for"
          },
          {
            "name": "rating",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Rating for this user"
          },
          {
            "name": "reviewerId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of the user reviewing"
          },
          {
            "name": "groupId",
            "in": "formData",
            "required": true,
            "type": "string",
            "description": "Id of the group peer participated in"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "//reviews",
              "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
              },
              "body": "peerId=&rating=&reviewerId=&groupId="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Review Create",
            "x-testDescription": "Create a new review for a group member"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///reviews": {
      "get": {
        "description": "Get reviews that belong to an author/user",
        "summary": "Review Get By Author/User",
        "tags": [
          "Review"
        ],
        "operationId": "ReviewsGet",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "userId",
            "in": "query",
            "required": true,
            "type": "string",
            "description": ""
          },
          {
            "name": "authorId",
            "in": "query",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "///reviews?userId=&authorId="
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Review Get By Author/User",
            "x-testDescription": "Get reviews that belong to an author/user"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "//reviews/": {
      "get": {
        "description": "List reviews",
        "summary": "Review Get",
        "tags": [
          "Review"
        ],
        "operationId": "ReviewsGet2",
        "produces": [
          "application/json"
        ],
        "parameters": [],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "//reviews/"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Review Get",
            "x-testDescription": "List reviews"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///reviews/{reviewId}": {
      "delete": {
        "description": "Delete an existing review by reviewId. Permissions: author",
        "summary": "Review Delete",
        "tags": [
          "Review"
        ],
        "operationId": "ReviewsByReviewIdDelete",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "reviewId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "DELETE",
              "uri": "///reviews/"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Review Delete",
            "x-testDescription": "Delete an existing review by reviewId. Permissions: author"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///merge/create/{InviterGroupId}/{InviteeGroupId}": {
      "post": {
        "description": "Creates a merger invite between two groups. Can be called by any group member.",
        "summary": "Merge Create",
        "tags": [
          "Merger"
        ],
        "operationId": "MergeCreateByInviterGroupIdAndInviteeGroupIdPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "InviterGroupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "Group Id of group requesting the merge"
          },
          {
            "name": "InviteeGroupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "Group Id of group that the request is sent to"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///merge/create//"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Merge Create",
            "x-testDescription": "Creates a merger invite between two groups. Can be called by any group member."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///merge/{MergerId}/accept": {
      "post": {
        "description": "Accepts a merger invite and merges the two groups into a new group. Can be called by any group member of the invitee group.",
        "summary": "Merge Accept",
        "tags": [
          "Merger"
        ],
        "operationId": "MergeAcceptByMergerIdPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "MergerId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "The id of the merger to accept"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///merge//accept"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Merge Accept",
            "x-testDescription": "Accepts a merger invite and merges the two groups into a new group. Can be called by any group member of the invitee group."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///merge/{MergerId}/reject": {
      "post": {
        "description": "Rejects a merger and deletes it. Can be called by any group member of the invitee group.",
        "summary": "Merge Reject",
        "tags": [
          "Merger"
        ],
        "operationId": "MergeRejectByMergerIdPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "MergerId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "The id of the merger to reject"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///merge//reject"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Merge Reject",
            "x-testDescription": "Rejects a merger and deletes it. Can be called by any group member of the invitee group."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///invite/create/{InviterGroupId}/{InviteeUserId}": {
      "post": {
        "description": "Creates an invite to invite a user to a group. Can be called by any group member.",
        "summary": "Invite Create",
        "tags": [
          "Invite"
        ],
        "operationId": "InviteCreateByInviterGroupIdAndInviteeUserIdPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "InviterGroupId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "Group Id of group sending the invite"
          },
          {
            "name": "InviteeUserId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "User Id of user that the request is sent to"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///invite/create//"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Invite Create",
            "x-testDescription": "Creates an invite to invite a user to a group. Can be called by any group member."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///invite/{InviteId}/accept": {
      "post": {
        "description": "Accepts an invite and adds the user to the group. Can be called by any group member.",
        "summary": "Invite Accept",
        "tags": [
          "Invite"
        ],
        "operationId": "InviteAcceptByInviteIdPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "InviteId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "The id of the invite"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///invite//accept"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Invite Accept",
            "x-testDescription": "Accepts an invite and adds the user to the group. Can be called by any group member."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "///invite/{InviteId}/reject": {
      "post": {
        "description": "Rejects an invite and deletes it. Can only be called by the user to whom it was sent.",
        "summary": "Invite Reject",
        "tags": [
          "Invite"
        ],
        "operationId": "InviteRejectByInviteIdPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "InviteId",
            "in": "path",
            "required": true,
            "type": "string",
            "description": "The id of the invite"
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "///invite//reject"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "Invite Reject",
            "x-testDescription": "Rejects an invite and deletes it. Can only be called by the user to whom it was sent."
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "//authenticate/login": {
      "post": {
        "description": "Authenticate user if username\\password combination is valid",
        "summary": "login",
        "tags": [
          "Authenticate"
        ],
        "operationId": "AuthenticateLoginPost",
        "produces": [
          "application/json"
        ],
        "parameters": [
          {
            "name": "login",
            "in": "header",
            "required": true,
            "type": "string",
            "description": ""
          },
          {
            "name": "password",
            "in": "header",
            "required": true,
            "type": "string",
            "description": ""
          }
        ],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "POST",
              "uri": "//authenticate/login",
              "headers": {
                "login": "",
                "password": ""
              }
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "login",
            "x-testDescription": "Authenticate user if username\\password combination is valid"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    },
    "//authenticate/logout": {
      "get": {
        "description": "Log out current user",
        "summary": "logout",
        "tags": [
          "Authenticate"
        ],
        "operationId": "AuthenticateLogoutGet",
        "produces": [
          "application/json"
        ],
        "parameters": [],
        "responses": {
          "200": {
            "description": ""
          }
        },
        "security": [],
        "x-unitTests": [
          {
            "request": {
              "method": "GET",
              "uri": "//authenticate/logout"
            },
            "expectedResponse": {
              "x-allowExtraHeaders": true,
              "x-bodyMatchMode": "NONE",
              "x-arrayOrderedMatching": false,
              "x-arrayCheckCount": false,
              "x-matchResponseSchema": true,
              "headers": {}
            },
            "x-testShouldPass": true,
            "x-testEnabled": true,
            "x-testName": "logout",
            "x-testDescription": "Log out current user"
          }
        ],
        "x-operation-settings": {
          "CollectParameters": false,
          "AllowDynamicQueryParameters": false,
          "AllowDynamicFormParameters": false,
          "IsMultiContentStreaming": false
        }
      }
    }
  }
}
