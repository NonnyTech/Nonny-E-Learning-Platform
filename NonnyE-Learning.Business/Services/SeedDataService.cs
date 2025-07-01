using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class SeedDataService: IHostedService
	{
		private readonly IServiceProvider _serviceProvider;

		public SeedDataService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			await SeedCoursesAsync(context);
			await SeedModulesAsync(context);
			await SeedQuizQuestionsAsync(context);
			await SeedPricingPlansAsync(context);
		}


		private async Task SeedCoursesAsync(ApplicationDbContext context)
		{
			var existingCourses = await context.Courses.ToListAsync();

			var courseList = new List<Course>
	{
		new Course
		{
			Category = "Web Programming",
			Title = "ASP.NET Core MVC",
			Description = "Build powerful web applications using ASP.NET Core MVC.",
			ImageUrl = "/upload/xcourse_01.png.pagespeed.ic.XTOvCuUmZu.png",
			Instructor = "Anthony Ikemefuna",
			Lectures = 23,
			Duration = "12 weeks",
			Price = 400000
		},
		new Course
		{
			Category = "Design",
			Title = "Graphics Design with Corel draw.",
			Description = "Master graphic design using Adobe Photoshop and Illustrator.",
			ImageUrl = "/upload/xcourse_02.png.pagespeed.ic.PL7Wu2UcSB.png",
			Instructor = "Rita Ikemefuna",
			Lectures = 23,
			Duration = "18 weeks",
			Price = 250000
		},
		new Course
		{
			Category = "Programming",
			Title = "C# for Beginners",
			Description = "Learn the fundamentals of C# programming.",
			ImageUrl = "/upload/xcourse_03.png.pagespeed.ic.8e1MyY5M7i.png",
			Instructor = "Stanley Nonso",
			Lectures = 23,
			Duration = "20 weeks",
			Price = 100000
		},
		new Course
		{
			Category = "Backend Development",
			Title = "Building Web APIs with .NET",
			Description = "Learn how to build scalable Web APIs using ASP.NET Core.",
			ImageUrl = "/upload/xcourse_04.png.pagespeed.ic.2rIKmUwjA7.png",
			Instructor = "Anthony Ikemefuna",
			Lectures = 23,
			Duration = "24 weeks",
			Price = 500000
		}
	};

			foreach (var newCourse in courseList)
			{
				var existing = existingCourses.FirstOrDefault(c => c.Title == newCourse.Title);
				if (existing != null)
				{
					// Update values
					existing.Category = newCourse.Category;
					existing.Description = newCourse.Description;
					existing.ImageUrl = newCourse.ImageUrl;
					existing.Instructor = newCourse.Instructor;
					existing.Lectures = newCourse.Lectures;
					existing.Duration = newCourse.Duration;
					existing.Price = newCourse.Price;
				}
				else
				{
					await context.Courses.AddAsync(newCourse);
				}
			}

			await context.SaveChangesAsync();
		}

		private async Task SeedModulesAsync(ApplicationDbContext context)
		{
			var csharpCourse = await context.Courses
				.FirstOrDefaultAsync(c => c.Title == "C# for Beginners");

			if (csharpCourse == null) return;

			var existingModules = await context.Modules
				.Where(m => m.CourseId == csharpCourse.CourseId)
				.ToListAsync();

			var newModules = new List<Module>
	{
		new Module { Title = "Introduction to C#", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 1 },
		new Module { Title = "Getting ready for C#", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 2 },
		new Module { Title = "The World of Variables and Operators", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 3 },
		new Module { Title = "Arrays, Strings and Lists", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 4 },
		new Module { Title = "Making our Program Interactive", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 5 },
		new Module { Title = "Making Choices and Decisions", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 6 },
		new Module { Title = "Object-Oriented Programming Part 1", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 7 },
		new Module { Title = "Object-Oriented Programming Part 2", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 8 },
		new Module { Title = "Enum and Struct", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 9 },
		new Module { Title = "LINQ", CourseContent = "We are one", CourseId = csharpCourse.CourseId, Order = 10 },
	};

			foreach (var module in newModules)
			{
				var existing = existingModules
					.FirstOrDefault(m => m.Order == module.Order && m.CourseId == module.CourseId);

				if (existing != null)
				{
					// Update existing
					existing.Title = module.Title;
					existing.CourseContent = module.CourseContent;
				}
				else
				{
					await context.Modules.AddAsync(module);
				}
			}

			await context.SaveChangesAsync();
		}

		private async Task SeedQuizQuestionsAsync(ApplicationDbContext context)
		{
			var allQuestionsByModule = new Dictionary<int, List<QuizQuestion>>
			{
				[1] = new List<QuizQuestion>
		{
			new QuizQuestion
			{   Order = 1,
				QuestionText = "What does C# stand for?",
				OptionA = "C Sharp",
				OptionB = "C Hash",
				OptionC = "See Hashtag",
				OptionD = "None of the above",
				CorrectOption = "A",
				ModuleId = 1
			},
			new QuizQuestion
			{
				Order = 2,
				QuestionText = "Who led the development of C#?",
				OptionA = "Bill Gates",
				OptionB = "Elon Musk",
				OptionC = "Anders Hejlsberg",
				OptionD = "Mark Zuckerberg",
				CorrectOption = "C",
				ModuleId = 1
			},
			new QuizQuestion
			{    Order = 3,
				QuestionText = "C# is part of which framework?",
				OptionA = "Java EE",
				OptionB = "Spring Boot",
				OptionC = "Angular",
				OptionD = ".NET",
				CorrectOption = "D",
				ModuleId = 1
			},
			new QuizQuestion
			{   Order = 4,
				QuestionText = "What type of programming language is C#?",
				OptionA = "Procedural",
				OptionB = "Object-Oriented",
				OptionC = "Functional",
				OptionD = "Assembly",
				CorrectOption = "B",
				ModuleId = 1
			},
			new QuizQuestion
			{   Order = 5,
				QuestionText = "Which of the following is NOT true about C#?",
				OptionA = "C# resembles English-like syntax",
				OptionB = "C# is very hard to learn",
				OptionC = "C# can be used for console applications",
				OptionD = "C# supports object-oriented programming",
				CorrectOption = "B",
				ModuleId = 1
			},
			new QuizQuestion
			{   Order = 6,
				QuestionText = "C# was created in which decade?",
				OptionA = "1980s",
				OptionB = "1990s",
				OptionC = "2000s",
				OptionD = "2010s",
				CorrectOption = "C",
				ModuleId = 1
			},
			new QuizQuestion
			{
				Order = 7,
				QuestionText = "Which of the following best describes the .NET framework?",
				OptionA = "A library of pre-written code",
				OptionB = "A front-end design tool",
				OptionC = "An AI engine",
				OptionD = "A hardware component",
				CorrectOption = "A",
				ModuleId = 1
			},
			new QuizQuestion
			{
				Order = 8,
				QuestionText = "Why is C# good for beginners?",
				OptionA = "It's similar to Assembly",
				OptionB = "It's overly complex",
				OptionC = "It's easy to learn and part of .NET",
				OptionD = "It lacks documentation",
				CorrectOption = "C",
				ModuleId = 1
			},
			new QuizQuestion
			{
				Order = 9,
				QuestionText = "Which of these applications CANNOT be built with C#?",
				OptionA = "Mobile apps",
				OptionB = "Web apps",
				OptionC = "Windows apps",
				OptionD = "C# cannot build any application",
				CorrectOption = "D",
				ModuleId = 1
			},
			new QuizQuestion
			{
				Order = 10,
				QuestionText = "What is the purpose of object-oriented programming?",
				OptionA = "To break code into small logical files",
				OptionB = "To represent programming problems as interacting objects",
				OptionC = "To avoid using variables",
				OptionD = "To only use HTML",
				CorrectOption = "B",
				ModuleId = 1
			}

		},

				[2] = new List<QuizQuestion>
		{

		new QuizQuestion
		{
			Order = 1,
			QuestionText = "What is Visual Studio Community (VSC)?",
			OptionA = "A web browser",
			OptionB = "A compiler only",
			OptionC = "An Integrated Development Environment (IDE)",
			OptionD = "A music player",
			CorrectOption = "C",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "What kind of application is 'HelloWorld'?",
			OptionA = "Web Application",
			OptionB = "Console Application",
			OptionC = "Mobile Application",
			OptionD = "Desktop GUI Application",
			CorrectOption = "B",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "Which method is the entry point of a C# console application?",
			OptionA = "Start()",
			OptionB = "Main()",
			OptionC = "Run()",
			OptionD = "Execute()",
			CorrectOption = "B",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "Which line in a C# program displays output on the screen?",
			OptionA = "Console.Display()",
			OptionB = "System.Show()",
			OptionC = "Console.WriteLine()",
			OptionD = "Print()",
			CorrectOption = "C",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "What does Console.Read() do?",
			OptionA = "Saves data to a file",
			OptionB = "Waits for a key press",
			OptionC = "Closes the application immediately",
			OptionD = "Prints a message",
			CorrectOption = "B",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "What is the purpose of 'using System;'?",
			OptionA = "To access Windows Registry",
			OptionB = "To include Java classes",
			OptionC = "To access core .NET classes like Console",
			OptionD = "To declare variables",
			CorrectOption = "C",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "What are comments in C# used for?",
			OptionA = "To write executable code",
			OptionB = "To increase speed",
			OptionC = "To document code for readability",
			OptionD = "To delete memory",
			CorrectOption = "C",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "Which of the following is a valid single-line comment?",
			OptionA = "-- This is a comment",
			OptionB = "/* This is a comment */",
			OptionC = "// This is a comment",
			OptionD = "# This is a comment",
			CorrectOption = "C",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "What symbol is used to start a multi-line comment?",
			OptionA = "//",
			OptionB = "#",
			OptionC = "/*",
			OptionD = "<!--",
			CorrectOption = "C",
			ModuleId = 2
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "Which folder should you save your projects in, according to the lesson?",
			OptionA = "C:\\Windows\\System32",
			OptionB = "Program Files",
			OptionC = "Desktop\\C# Projects",
			OptionD = "Recycle Bin",
			CorrectOption = "C",
			ModuleId = 2
		}

		},

				[3] = new List<QuizQuestion>
				{
			new QuizQuestion
			{
				Order = 1,
				QuestionText = "Which of the following is a valid way to declare a variable that stores a user's age in C#?",
				OptionA = "int age = \"25\";",
				OptionB = "age int = 25;",
				OptionC = "int userAge = 25;",
				OptionD = "25 = int userAge;",
				CorrectOption = "C",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 2,
				QuestionText = "What is the default data type for decimal numbers in C#?",
				OptionA = "float",
				OptionB = "decimal",
				OptionC = "double",
				OptionD = "int",
				CorrectOption = "C",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 3,
				QuestionText = "What will be the output of: int x = 7, y = 2; Console.WriteLine(x / y);",
				OptionA = "3.5",
				OptionB = "3",
				OptionC = "0",
				OptionD = "3.0",
				CorrectOption = "B",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 4,
				QuestionText = "Which data type should you use if you want the highest precision for a decimal number in C#?",
				OptionA = "float",
				OptionB = "int",
				OptionC = "double",
				OptionD = "decimal",
				CorrectOption = "D",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 5,
				QuestionText = "Which symbol is used for the modulus operator in C#?",
				OptionA = "/",
				OptionB = "*",
				OptionC = "%",
				OptionD = "^",
				CorrectOption = "C",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 6,
				QuestionText = "What is the value of x after: int x = 5; x += 3;",
				OptionA = "3",
				OptionB = "8",
				OptionC = "5",
				OptionD = "53",
				CorrectOption = "B",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 7,
				QuestionText = "Which of the following is NOT a valid variable name in C#?",
				OptionA = "user_name",
				OptionB = "userName2",
				OptionC = "_userName",
				OptionD = "2userName",
				CorrectOption = "D",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 8,
				QuestionText = "What does the ++ operator do in C#?",
				OptionA = "Adds 2 to a variable",
				OptionB = "Increases a variable's value by 1",
				OptionC = "Multiplies a variable by 2",
				OptionD = "Resets a variable to 0",
				CorrectOption = "B",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 9,
				QuestionText = "What will the following code output? int counter = 5; Console.WriteLine(counter++);",
				OptionA = "6",
				OptionB = "5",
				OptionC = "0",
				OptionD = "Compile error",
				CorrectOption = "B",
				ModuleId = 3
			},
			new QuizQuestion
			{
				Order = 10,
				QuestionText = "How do you explicitly cast a double value 20.9 to an int in C#?",
				OptionA = "int x = int(20.9);",
				OptionB = "int x = (int) 20.9;",
				OptionC = "int x = 20.9f;",
				OptionD = "int x = \"20.9\";",
				CorrectOption = "B",
				ModuleId = 3
			}


				},
				[4] = new List<QuizQuestion>
				{
		new QuizQuestion
		{
			Order = 1,
			QuestionText = "How do you declare and initialize an array of integers in C# with 5 elements?",
			OptionA = "int[] userAge = {21, 22, 23, 24, 25};",
			OptionB = "int userAge[] = new int[5];",
			OptionC = "int userAge[] = {21, 22, 23, 24, 25};",
			OptionD = "int userAge = new int(5);",
			CorrectOption = "A",
			ModuleId = 4
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "What is the default value of an integer array element in C# if not initialized?",
			OptionA = "0",
			OptionB = "null",
			OptionC = "undefined",
			OptionD = "NaN",
			CorrectOption = "A",
			ModuleId = 4
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "Which method is used to sort an array in C#?",
			OptionA = "Array.Order()",
			OptionB = "Array.Sort()",
			OptionC = "Array.Reverse()",
			OptionD = "Array.IndexOf()",
			CorrectOption = "B",
			ModuleId =4
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "What will be the output of: int[] numbers = {10, 30, 44, 21, 51}; Console.WriteLine(Array.IndexOf(numbers, 21));",
			OptionA = "-1",
			OptionB = "3",
			OptionC = "0",
			OptionD = "4",
			CorrectOption = "B",
			ModuleId = 4
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "How do you declare a string in C#?",
			OptionA = "string message = 'Hello World';",
			OptionB = "string message = \"Hello World\";",
			OptionC = "string message = Hello World;",
			OptionD = "string message = new string('Hello World');",
			CorrectOption = "B",
			ModuleId = 4
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "Which property of a string gives the total number of characters it contains?",
			OptionA = "Size",
			OptionB = "Length",
			OptionC = "Count",
			OptionD = "TotalCount",
			CorrectOption = "B",
			ModuleId =4
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "Which method is used to check if two strings are identical in C#?",
			OptionA = "Equals()",
			OptionB = "Compare()",
			OptionC = "IsEqual()",
			OptionD = "AreEqual()",
			CorrectOption = "A",
			ModuleId =4
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "Which method would you use to add an element to the end of a list in C#?",
			OptionA = "Add()",
			OptionB = "Insert()",
			OptionC = "Push()",
			OptionD = "Append()",
			CorrectOption = "A",
			ModuleId =4
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "How do you remove the first occurrence of an element from a list in C#?",
			OptionA = "Delete()",
			OptionB = "Remove()",
			OptionC = "RemoveAt()",
			OptionD = "Clear()",
			CorrectOption = "B",
			ModuleId = 4
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "Which method would you use to check if a list contains a specific element in C#?",
			OptionA = "Includes()",
			OptionB = "Exists()",
			OptionC = "Contains()",
			OptionD = "HasElement()",
			CorrectOption = "C",
			ModuleId =4
		}


				},
				[5] = new List<QuizQuestion>

				{
				new QuizQuestion
		{
			Order = 1,
			QuestionText = "Which method in C# is used to display a message on the screen and move the cursor to the next line?",
			OptionA = "Console.Write()",
			OptionB = "Console.WriteLine()",
			OptionC = "Console.Print()",
			OptionD = "Console.PrintLine()",
			CorrectOption = "B",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "What is the difference between Console.Write() and Console.WriteLine() in C#?",
			OptionA = "Console.WriteLine() does not add a new line after the message.",
			OptionB = "Console.Write() adds a new line after the message.",
			OptionC = "Console.WriteLine() adds a new line after the message.",
			OptionD = "There is no difference between Console.Write() and Console.WriteLine().",
			CorrectOption = "C",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "Which of the following correctly displays the value of the variable userAge in C#?",
			OptionA = "Console.WriteLine(userAge);",
			OptionB = "Console.Write(userAge);",
			OptionC = "Console.Print(userAge);",
			OptionD = "Console.WriteLine(‘userAge’);",
			CorrectOption = "A",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "Which operator is used to concatenate strings in C#?",
			OptionA = "&",
			OptionB = "+",
			OptionC = "%",
			OptionD = ".",
			CorrectOption = "B",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "If we have the following code: int results = 79; Console.WriteLine('You scored ' + results + ' marks for your test.'); What will be the output?",
			OptionA = "You scored results marks for your test.",
			OptionB = "You scored 79 marks for your test.",
			OptionC = "You scored 'results' marks for your test.",
			OptionD = "Error: Cannot concatenate string and integer.",
			CorrectOption = "B",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "Which placeholder format is used in Console.WriteLine() for displaying variables?",
			OptionA = "{0}, {1}, etc.",
			OptionB = "%0, %1, etc.",
			OptionC = "[0], [1], etc.",
			OptionD = "(0), (1), etc.",
			CorrectOption = "A",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "Which method reads an entire line of text input from the user in C#?",
			OptionA = "Read()",
			OptionB = "ReadLine()",
			OptionC = "Input()",
			OptionD = "GetLine()",
			CorrectOption = "B",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "What is the output of the following code: string userInput = Console.ReadLine(); Console.WriteLine(userInput); if the user enters 'Hello'?",
			OptionA = "Hello",
			OptionB = "userInput",
			OptionC = "Error",
			OptionD = "Hello World",
			CorrectOption = "A",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "What is the C# method used to convert a string to an integer?",
			OptionA = "Convert.ToString()",
			OptionB = "Convert.ToInt32()",
			OptionC = "Convert.ToFloat()",
			OptionD = "Convert.ToDecimal()",
			CorrectOption = "B",
			ModuleId = 5
		},
		new QuizQuestion
		{
			Order = 0,
			QuestionText = "Which method would you use to check if two strings are identical in C#?",
			OptionA = "Equals()",
			OptionB = "Compare()",
			OptionC = "IsEqual()",
			OptionD = "AreEqual()",
			CorrectOption = "A",
			ModuleId = 5
		}


				},
				[6] = new List<QuizQuestion>

				{

		new QuizQuestion
		{
			Order = 1,
			QuestionText = "What is the purpose of the 'using' keyword in C#?",
			OptionA = "To define a namespace",
			OptionB = "To include an external library",
			OptionC = "To create a new instance of a class",
			OptionD = "To mark a variable as unused",
			CorrectOption = "B",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "Which of the following is used to declare a constant variable in C#?",
			OptionA = "const",
			OptionB = "final",
			OptionC = "static",
			OptionD = "readonly",
			CorrectOption = "A",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "Which method is used to parse a string into a double in C#?",
			OptionA = "Double.Parse()",
			OptionB = "Convert.ToDouble()",
			OptionC = "ParseDouble()",
			OptionD = "ToDouble.Parse()",
			CorrectOption = "A",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "In C#, what is the default value of a boolean variable?",
			OptionA = "True",
			OptionB = "False",
			OptionC = "Null",
			OptionD = "0",
			CorrectOption = "B",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "Which of the following is not a valid data type in C#?",
			OptionA = "int",
			OptionB = "string",
			OptionC = "bool",
			OptionD = "real",
			CorrectOption = "D",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "Which operator is used to check equality between two values in C#?",
			OptionA = "==",
			OptionB = "=",
			OptionC = "===",
			OptionD = "!=",
			CorrectOption = "A",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "What is the output of the following code: Console.WriteLine(5 / 2);",
			OptionA = "2.5",
			OptionB = "2",
			OptionC = "Error",
			OptionD = "5",
			CorrectOption = "B",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "How do you declare an array of integers in C#?",
			OptionA = "int[] arr;",
			OptionB = "array int[];",
			OptionC = "int arr[];",
			OptionD = "int arr[10];",
			CorrectOption = "A",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "Which of the following C# data types can store a decimal number?",
			OptionA = "int",
			OptionB = "bool",
			OptionC = "float",
			OptionD = "char",
			CorrectOption = "C",
			ModuleId = 6
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "What does the 'break' statement do in a loop in C#?",
			OptionA = "Terminates the current iteration of the loop",
			OptionB = "Exits the loop completely",
			OptionC = "Skips the rest of the current iteration",
			OptionD = "Resumes the loop from the beginning",
			CorrectOption = "B",
			ModuleId = 6
		}


				},
				[7] = new List<QuizQuestion>


				{
				new QuizQuestion
		{
			Order = 1,
			QuestionText = "What does the term 'Object-Oriented Programming' (OOP) mean?",
			OptionA = "An approach to programming that uses only objects",
			OptionB = "A method of breaking a program into functions",
			OptionC = "A programming paradigm that divides problems into objects that interact with each other",
			OptionD = "An approach that uses no classes or objects",
			CorrectOption = "C",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "Which of the following is true about classes and objects in OOP?",
			OptionA = "An object is the blueprint of a class",
			OptionB = "An object is created from a class template",
			OptionC = "A class cannot have constructors",
			OptionD = "A class is an instance of an object",
			CorrectOption = "B",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "What is the purpose of a constructor in a class?",
			OptionA = "To initialize the fields of the class when an object is created",
			OptionB = "To display output to the user",
			OptionC = "To calculate the pay of a staff member",
			OptionD = "To define methods within the class",
			CorrectOption = "A",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "What is the default constructor in C# used for?",
			OptionA = "It initializes the fields to custom values",
			OptionB = "It initializes the fields to default values (zero for numbers and empty for strings)",
			OptionC = "It is used for only static fields",
			OptionD = "It is used to delete the object after use",
			CorrectOption = "B",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "Which access modifier would you use to ensure that a field is only accessible from within the class?",
			OptionA = "public",
			OptionB = "private",
			OptionC = "protected",
			OptionD = "internal",
			CorrectOption = "B",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "What does the 'const' keyword do when applied to a class field?",
			OptionA = "It makes the field public",
			OptionB = "It ensures the field can be modified only by the constructor",
			OptionC = "It indicates that the field’s value cannot be changed after initialization",
			OptionD = "It makes the field static",
			CorrectOption = "C",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "Which of the following is true about static members of a class?",
			OptionA = "Static members can only be accessed through instances of the class",
			OptionB = "Static members are accessed through the class name rather than through an object",
			OptionC = "Static members are only used for constructors",
			OptionD = "Static members can only store numbers",
			CorrectOption = "B",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "In the Staff class example, which method is responsible for calculating the salary?",
			OptionA = "PrintMessage()",
			OptionB = "ToString()",
			OptionC = "CalculatePay()",
			OptionD = "CalculatePay(int bonus, int allowance)",
			CorrectOption = "C",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "Which of the following is a correct example of instantiating an object from a class?",
			OptionA = "Staff staff1 = Staff();",
			OptionB = "Staff staff1 = new Staff('Peter');",
			OptionC = "new Staff staff1 = new Staff('Peter');",
			OptionD = "Staff staff1 = new Staff();",
			CorrectOption = "D",
			ModuleId = 7
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "What is encapsulation in OOP?",
			OptionA = "A method of organizing classes and objects based on functionality",
			OptionB = "Hiding the internal state of an object and exposing only necessary functionality",
			OptionC = "Allowing classes to inherit functionality from another class",
			OptionD = "Ensuring all methods are static",
			CorrectOption = "B",
			ModuleId = 7
		}

				},
				[8] = new List<QuizQuestion>


				{
		new QuizQuestion
		{
			Order = 1,
			QuestionText = "What is inheritance in C#?",
			OptionA = "A process of hiding class details",
			OptionB = "A mechanism to create new classes from existing ones",
			OptionC = "A method of copying objects",
			OptionD = "A feature that restricts object creation",
			CorrectOption = "B",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "Which keyword is used in C# to inherit from a base class?",
			OptionA = "inherits",
			OptionB = "base",
			OptionC = ":",
			OptionD = "extends",
			CorrectOption = "C",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "What does the 'virtual' keyword indicate in C#?",
			OptionA = "That the method must be overridden",
			OptionB = "That the method can be overridden in a derived class",
			OptionC = "That the method cannot be accessed outside the class",
			OptionD = "That the method is abstract",
			CorrectOption = "B",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "How do you override a method from the base class in C#?",
			OptionA = "Use the 'new' keyword",
			OptionB = "Use the 'override' keyword",
			OptionC = "Use the 'virtual' keyword",
			OptionD = "Use the 'abstract' keyword",
			CorrectOption = "B",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "What is polymorphism in C#?",
			OptionA = "Storing multiple objects in one variable",
			OptionB = "The ability of a class to inherit multiple classes",
			OptionC = "The ability of different classes to be treated as instances of the same base class",
			OptionD = "A way to encapsulate objects",
			CorrectOption = "C",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "Which of the following allows method overloading in C#?",
			OptionA = "Methods with different return types",
			OptionB = "Methods with the same name but different parameters",
			OptionC = "Methods with the same name and same parameters",
			OptionD = "Methods in different classes",
			CorrectOption = "B",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "What is abstraction in object-oriented programming?",
			OptionA = "Exposing all details of the class",
			OptionB = "Providing simple interfaces while hiding complex implementation",
			OptionC = "Combining multiple classes into one",
			OptionD = "Inheritance from multiple classes",
			CorrectOption = "B",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "Which keyword is used to define an abstract class in C#?",
			OptionA = "abstract",
			OptionB = "base",
			OptionC = "virtual",
			OptionD = "override",
			CorrectOption = "A",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "Which statement is true about abstract methods?",
			OptionA = "They can have a body",
			OptionB = "They must be defined in non-abstract classes",
			OptionC = "They are implemented in the derived class",
			OptionD = "They cannot be inherited",
			CorrectOption = "C",
			ModuleId = 8
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "Can a class inherit from multiple base classes in C#?",
			OptionA = "Yes, using the 'extends' keyword",
			OptionB = "Yes, using interfaces",
			OptionC = "No, C# does not support any form of multiple inheritance",
			OptionD = "Yes, by chaining classes",
			CorrectOption = "B",
			ModuleId = 8
		}


				},

				[9] = new List<QuizQuestion>
				{
		new QuizQuestion
		{
			Order = 1,
			QuestionText = "What is an interface in C#?",
			OptionA = "A class with abstract methods only",
			OptionB = "A class that provides implementation details",
			OptionC = "A contract that defines method signatures without implementation",
			OptionD = "A static class with utility methods",
			CorrectOption = "C",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "Which keyword is used to implement an interface in C#?",
			OptionA = "extends",
			OptionB = "inherits",
			OptionC = "interface",
			OptionD = ":",
			CorrectOption = "D",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "What is a delegate in C#?",
			OptionA = "A class that holds integers",
			OptionB = "A reference type that holds reference to a method",
			OptionC = "An abstract class with virtual methods",
			OptionD = "A method that implements an interface",
			CorrectOption = "B",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "Which keyword is used to declare a delegate?",
			OptionA = "event",
			OptionB = "delegate",
			OptionC = "function",
			OptionD = "method",
			CorrectOption = "B",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "What are events in C#?",
			OptionA = "Variables used in exception handling",
			OptionB = "Methods that override delegates",
			OptionC = "Special delegates used for notifications",
			OptionD = "Interfaces with abstract methods",
			CorrectOption = "C",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "Which keyword is used to declare an event in C#?",
			OptionA = "delegate",
			OptionB = "event",
			OptionC = "trigger",
			OptionD = "signal",
			CorrectOption = "B",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "Which block is used to handle exceptions in C#?",
			OptionA = "try-catch",
			OptionB = "if-else",
			OptionC = "switch-case",
			OptionD = "loop-do",
			CorrectOption = "A",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "What does the 'finally' block do in exception handling?",
			OptionA = "Executes only when there is no exception",
			OptionB = "Executes only if an exception occurs",
			OptionC = "Executes regardless of whether an exception occurs or not",
			OptionD = "Skips error handling",
			CorrectOption = "C",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "How do you throw a custom exception in C#?",
			OptionA = "throw new Exception();",
			OptionB = "raise Exception();",
			OptionC = "error Exception();",
			OptionD = "catch Exception();",
			CorrectOption = "A",
			ModuleId = 9
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "Which of the following is true about interfaces?",
			OptionA = "They can contain method bodies",
			OptionB = "They cannot be implemented by classes",
			OptionC = "They define a contract without implementation",
			OptionD = "They are used to inherit multiple classes",
			CorrectOption = "C",
			ModuleId = 9
		}

				},
				[10] = new List<QuizQuestion>

				{
		new QuizQuestion
		{
			Order = 1,
			QuestionText = "What does LINQ stand for in C#?",
			OptionA = "Linked Information Query",
			OptionB = "Language-Integrated Query",
			OptionC = "Line Information Query",
			OptionD = "Logical Internal Query",
			CorrectOption = "B",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 2,
			QuestionText = "Which keyword starts a LINQ query in query syntax?",
			OptionA = "select",
			OptionB = "orderby",
			OptionC = "from",
			OptionD = "where",
			CorrectOption = "C",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 3,
			QuestionText = "What does the 'where' clause do in a LINQ query?",
			OptionA = "Sorts the data",
			OptionB = "Filters the data",
			OptionC = "Creates new objects",
			OptionD = "Specifies the output format",
			CorrectOption = "B",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 4,
			QuestionText = "What is the output type of a LINQ query stored using 'var'?",
			OptionA = "string",
			OptionB = "List",
			OptionC = "Automatically inferred by the compiler",
			OptionD = "int[]",
			CorrectOption = "C",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 5,
			QuestionText = "What will the following LINQ query return? from num in numbers where (num % 2) == 0 select num;",
			OptionA = "All odd numbers",
			OptionB = "All numbers",
			OptionC = "All even numbers",
			OptionD = "Numbers divisible by 3",
			CorrectOption = "C",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 6,
			QuestionText = "Which LINQ clause is used to arrange results?",
			OptionA = "orderby",
			OptionB = "groupby",
			OptionC = "select",
			OptionD = "join",
			CorrectOption = "A",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 7,
			QuestionText = "Which keyword is used to create anonymous types in a LINQ query?",
			OptionA = "object",
			OptionB = "var",
			OptionC = "dynamic",
			OptionD = "new",
			CorrectOption = "D",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 8,
			QuestionText = "Which of these LINQ queries retrieves customers with negative balances?",
			OptionA = "where cust.Balance > 0",
			OptionB = "where cust.Balance < 0",
			OptionC = "where cust.Balance == 0",
			OptionD = "where cust.Balance >= 0",
			CorrectOption = "B",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 9,
			QuestionText = "What is the purpose of the 'select' clause in LINQ?",
			OptionA = "To modify the original data",
			OptionB = "To perform calculations",
			OptionC = "To choose what to include in the result",
			OptionD = "To remove duplicates",
			CorrectOption = "C",
			ModuleId = 10
		},
		new QuizQuestion
		{
			Order = 10,
			QuestionText = "What type of loop is used to execute a LINQ query result?",
			OptionA = "for",
			OptionB = "do-while",
			OptionC = "foreach",
			OptionD = "while",
			CorrectOption = "C",
			ModuleId = 10
		}
				}

				};

			foreach (var moduleEntry in allQuestionsByModule)
			{
				int moduleId = moduleEntry.Key;
				var newQuestions = moduleEntry.Value;

				var existingQuestions = await context.QuizQuestions
					.Where(q => q.ModuleId == moduleId)
					.ToListAsync();

				foreach (var newQuestion in newQuestions)
				{
					var existing = existingQuestions
						.FirstOrDefault(q => q.Order == newQuestion.Order);

					if (existing != null)
					{
						existing.QuestionText = newQuestion.QuestionText;
						existing.OptionA = newQuestion.OptionA;
						existing.OptionB = newQuestion.OptionB;
						existing.OptionC = newQuestion.OptionC;
						existing.OptionD = newQuestion.OptionD;
						existing.CorrectOption = newQuestion.CorrectOption;
					}
					else
					{
						await context.QuizQuestions.AddAsync(newQuestion);
					}
				}
			}

			await context.SaveChangesAsync();
		}

		private static async Task SeedPricingPlansAsync(ApplicationDbContext context)
		{
			if (context.PricingPlans.Any())
				return; // Plans already exist, no need to seed again

			var plans = new List<PricingPlan>
	{
		new PricingPlan
		{
			PlanName = "Foundation",
			Price = 60000,
			Description = "Intro to C# & MVC"
		},
		new PricingPlan
		{
			PlanName = "Beginner",
			Price = 80000,
			Description = "Core C# & Basic MVC"
		},
		new PricingPlan
		{
			PlanName = "Intermediate",
			Price = 120000,
			Description = "Real-World C# & MVC"
		}
	};

			await context.PricingPlans.AddRangeAsync(plans);
			await context.SaveChangesAsync();
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
