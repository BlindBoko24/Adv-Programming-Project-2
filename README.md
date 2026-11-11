# Course Planner (Project 2)

A C# console application that helps students:

1. Validate that a plan they create satisfies the course requirements for a major in a given year.
2. Generate a full valid plan for their major that includes any courses they have already taken.

## How it works

1. The user chooses to validate a plan or generate a plan.
2. **Validate mode:** the user enters a plan and the validator answers yes or no, with a reason for any no.
3. **Generate mode:**
   - The list generator builds a minimal course list for the major, ignoring semesters.
   - The validator checks that the list covers all requirements and meets the required credits.
   - The schedule generator orders the list into semesters.
   - If a course cannot be placed, it is sent back to the list generator as an exclusion, and a new list is generated (capped at a fixed number of retries).
4. The final plan is printed in the output format provided by Gilmore.

## Team

| Role | Person |
|---|---|
| Person A: console, data loading and integration (Git lead) | Jaime |
| Person B: validator | Noah T |
| Person C: list generator | Fernando |
| Person D: schedule generator | Noah R |

## Assignments

### Person A: console, data loading and integration (Git lead) - Jaime

- Build the menu in `Program.cs` with two options: validate a plan, or generate a plan.
- Read the user's plan input and print results in Gilmore's output format.
- Write the retry loop that connects the three engines (cap it at a fixed number of retries).
- Fix the repository to match the real database tables, and load the major requirements from the converter's output.
- Merge everyone's work.

### Person B: validator - Noah T

- `ValidateList` checks that the course list covers all the major's requirements and meets the required credits. The list generator and the retry loop both use it.
- `ValidatePlan` does everything `ValidateList` does, plus checks that prerequisites come earlier, courses are offered in that term, and nothing is duplicated. Both return yes/no, with the reason for any "no".
- Write the test cases, since this piece also acts as the grader for everyone else's output.

### Person C: list generator - Fernando

- Input: the major, the courses already taken and the excluded courses.
- Output: a course list that covers all requirements with as few courses as possible and enough credits.
- Start with a simple greedy approach: take the required courses first, then fill the remaining electives and credits with the fewest courses. Ignore semesters completely.
- Never include an excluded course.

### Person D: schedule generator - Noah R

- Input: a valid course list.
- Output: courses placed into semesters, respecting prerequisites, the terms each course is offered, and a credit limit per semester.
- Start with a simple approach: repeatedly place every course whose prerequisites are done into the next semester.
- If a course can't be placed, return it as the failed course so the loop can exclude it.
- Call Person B's `ValidatePlan` as a final check.

## Data

- Course data comes from the SQLite database `CourseInformation.sqlite`.
- Major requirements come from the cleaned-up professor spreadsheets, exported to CSV and stored in the repository.
- Courses are identified everywhere by their course code in the format `ME-2222`.

## Git rules

- Each person works only in their own files or folder to avoid merge conflicts.
- The shared model classes are owned by Person A. Ask Person A before changing them.
- Work on your own branch (for example `validator`, `list-generator`, `scheduler`) and merge through a pull request. Person A reviews.
- Make small commits with clear messages.
- Until the real data loading is finished, test your piece with hardcoded sample majors and courses.
