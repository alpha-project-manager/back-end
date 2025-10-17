using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_questions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    msg_text = table.Column<string>(type: "text", nullable: false),
                    prev_question_id = table.Column<Guid>(type: "uuid", nullable: true),
                    next_question_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_questions_application_questions_next_question_id",
                        column: x => x.next_question_id,
                        principalTable: "application_questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_application_questions_application_questions_prev_question_id",
                        column: x => x.prev_question_id,
                        principalTable: "application_questions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "calendar_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_url = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "control_points",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_points", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tutors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    patronymic = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tutors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    patronymic = table.Column<string>(type: "text", nullable: true),
                    academic_group = table.Column<string>(type: "text", nullable: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_students", x => x.id);
                    table.ForeignKey(
                        name: "fk_students_student_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "student_roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "project_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    goal = table.Column<string>(type: "text", nullable: false),
                    requested_result = table.Column<string>(type: "text", nullable: false),
                    criteria = table.Column<string>(type: "text", nullable: false),
                    tutor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    max_teams = table.Column<int>(type: "integer", nullable: false),
                    accepted_teams = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_cases", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_cases_tutors_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "tutors",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    salt = table.Column<string>(type: "text", nullable: false),
                    calendar_settings_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tutor_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_calendar_settings_calendar_settings_id",
                        column: x => x.calendar_settings_id,
                        principalTable: "calendar_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_tutors_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "tutors",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "project_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_title = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_username = table.Column<string>(type: "text", nullable: false),
                    current_question_id = table.Column<Guid>(type: "uuid", nullable: true),
                    next_question_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_applications_application_questions_next_question_id",
                        column: x => x.next_question_id,
                        principalTable: "application_questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_project_applications_project_cases_case_id",
                        column: x => x.case_id,
                        principalTable: "project_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    tutor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    meeting_url = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    semester = table.Column<int>(type: "integer", nullable: false),
                    academic_year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_project_cases_case_id",
                        column: x => x.case_id,
                        principalTable: "project_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_projects_tutors_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "tutors",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "application_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_messages_project_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "project_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_question_answers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_title = table.Column<string>(type: "text", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_question_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_question_answers_project_applications_applicati",
                        column: x => x.application_id,
                        principalTable: "project_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "control_point_in_projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    control_point_id = table.Column<Guid>(type: "uuid", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    video_url = table.Column<string>(type: "text", nullable: false),
                    company_mark = table.Column<int>(type: "integer", nullable: false),
                    urfu_mark = table.Column<int>(type: "integer", nullable: false),
                    completed = table.Column<bool>(type: "boolean", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_point_in_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_control_point_in_projects_control_points_control_point_id",
                        column: x => x.control_point_id,
                        principalTable: "control_points",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_control_point_in_projects_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meetings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    result_mark = table.Column<int>(type: "integer", nullable: true),
                    is_finished = table.Column<bool>(type: "boolean", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meetings", x => x.id);
                    table.ForeignKey(
                        name: "fk_meetings_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_in_projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_in_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_in_projects_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_student_in_projects_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "todo_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    meeting_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_todo_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_todo_tasks_meetings_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "meetings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_application_messages_application_id",
                table: "application_messages",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_question_answers_application_id",
                table: "application_question_answers",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_questions_next_question_id",
                table: "application_questions",
                column: "next_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_questions_prev_question_id",
                table: "application_questions",
                column: "prev_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_control_point_in_projects_control_point_id",
                table: "control_point_in_projects",
                column: "control_point_id");

            migrationBuilder.CreateIndex(
                name: "ix_control_point_in_projects_project_id",
                table: "control_point_in_projects",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_meetings_project_id",
                table: "meetings",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_applications_case_id",
                table: "project_applications",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_applications_next_question_id",
                table: "project_applications",
                column: "next_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_cases_tutor_id",
                table: "project_cases",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_case_id",
                table: "projects",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_tutor_id",
                table: "projects",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_in_projects_project_id",
                table: "student_in_projects",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_in_projects_student_id",
                table: "student_in_projects",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_students_role_id",
                table: "students",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_todo_tasks_meeting_id",
                table: "todo_tasks",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_calendar_settings_id",
                table: "users",
                column: "calendar_settings_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_tutor_id",
                table: "users",
                column: "tutor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_messages");

            migrationBuilder.DropTable(
                name: "application_question_answers");

            migrationBuilder.DropTable(
                name: "control_point_in_projects");

            migrationBuilder.DropTable(
                name: "student_in_projects");

            migrationBuilder.DropTable(
                name: "todo_tasks");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "project_applications");

            migrationBuilder.DropTable(
                name: "control_points");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "meetings");

            migrationBuilder.DropTable(
                name: "calendar_settings");

            migrationBuilder.DropTable(
                name: "application_questions");

            migrationBuilder.DropTable(
                name: "student_roles");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "project_cases");

            migrationBuilder.DropTable(
                name: "tutors");
        }
    }
}
