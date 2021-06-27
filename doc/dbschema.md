## User
	Name
	Email
	Password <- Plain text!
	GlobalRoles: [ Admin, CourseCreator] # None, one or both
	Invites: [ {REF_GROUP:status}] # Need to sync with Groups!
	Courses: [REF_COURSE]	#Need to sync with Groups!

## Course
	Name
	SkillSet: [ {Skill: Type}]
	Preferences: {
		GroupSize: {
			min: minVal
			max: maxVal
		}
		StartDate
		EndDate
	}
	Enrollments: [ {REF_USER: [skill: value] }]
	Groups: [REF_GROUP]

## Group
	Name
	Bio
	Members: [REF_USER]
	Invites: [ {REF_USER: status} ] # Suggestions are not here!
								# They are generated on-the-fly
	Mergers: [ {REF_GROUP: status} ]
	DesiredSkills: [ {Skill:value} ]


