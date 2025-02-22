# Task Tracker

A simple task tracking application built following the [roadmap.sh project guide](https://roadmap.sh/projects/task-tracker). This project helps users manage their tasks efficiently with features like adding, updating, deleting, and marking tasks.

## Features

- List tasks with the `list` command.
- Add tasks to task list with the `add` command.
- Updata tasks with the `update` command.
- Change status of the tasks with the `mark` command.

### How It's Work

When you use the `ttracker` command, it creates a `.tasks.json` file in the folder you're working in. The task tracker saves your tasks to this `.tasks.json` file.

> [!IMPORTANT]
> This project is provided "as-is" without any guarantees or warranties. The developer is not responsible for any data loss, system damage, or other issues that may arise from using this application. Users are encouraged to back up their data regularly and use the application at their own risk.

## Installation

To install the Task Tracker, follow these steps:

### For Linux (.deb)

**1.** Download the '.deb'  package from the releases page.

**2.** Install the package using the following command:

```bash
sudo dpkg -i task-tracker.deb
```

### For Windows (.msi)

**1.** Download the `.msi` installer from the releases page.

**2.** Run the installer and follow the on-screen instructions to complete the installation.

## Usage

tasks can take `todo`, `in-progress` and `done` status. It can also take hidden `deleted` status.

- `ttracker list`: Displays all tasks in the list.
- `ttracker list in-progress`: Displays all tasks with the in-progress status.
- `ttracker add "task title"`: Adds a new task to your list.
- `ttracker update <task-id> "updated task title"`: Updates the description of an existing task with the given task ID.
- `ttracker mark <task-id> todo`: Marks a task as "To Do".
- `ttracker mark <task-id> in-progress`: Marks a task as "In Progress".
- `ttracker mark <task-id> done`: Marks a task as "Done".

## License

This project is licensed under the [MIT License](https://mit-license.org/).
