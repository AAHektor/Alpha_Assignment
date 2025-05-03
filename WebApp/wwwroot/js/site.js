document.addEventListener('DOMContentLoaded', () => {

    // -------------------------------
    // Initiera alla Quill-redigerare
    // -------------------------------
    document.querySelectorAll('[data-quill-editor]').forEach(editorContainer => {
        const editorId = editorContainer.getAttribute('data-quill-editor');
        const toolbarId = editorContainer.getAttribute('data-quill-toolbar');
        const textareaId = editorContainer.getAttribute('data-quill-target');

        const quill = new Quill(`#${editorId}`, {
            modules: { toolbar: `#${toolbarId}` },
            theme: 'snow',
            placeholder: 'Type something'
        });

        const textarea = document.getElementById(textareaId);
        if (textarea) {
            quill.on('text-change', () => {
                textarea.value = quill.root.innerHTML;
            });
        }
    });

    // -------------------------------
    // Dropdown-knappar (account, settings etc.)
    // -------------------------------
    document.querySelectorAll('[data-type="dropdown"]').forEach(button => {
        button.addEventListener('click', e => {
            e.stopPropagation();
            const target = document.querySelector(button.getAttribute('data-target'));

            document.querySelectorAll('.dropdown').forEach(dropdown => {
                if (dropdown !== target) dropdown.classList.remove('show');
            });

            if (target) target.classList.toggle('show');
        });
    });

    // -------------------------------
    // Klick utanför = stäng dropdowns
    // -------------------------------
    document.addEventListener('click', () => {
        document.querySelectorAll('.dropdown').forEach(dropdown => {
            dropdown.classList.remove('show');
        });
    });

    // -------------------------------
    // Öppna alla modaler
    // -------------------------------
    document.querySelectorAll('[data-type="modal"]').forEach(button => {
        button.addEventListener('click', () => {
            const modal = document.querySelector(button.getAttribute('data-target'));
            if (modal) modal.classList.add('show');
        });
    });

    // -------------------------------
    // Stäng alla modaler
    // -------------------------------
    document.querySelectorAll('[data-type="close"]').forEach(button => {
        button.addEventListener('click', () => {
            const modal = document.querySelector(button.getAttribute('data-target'));
            if (modal) modal.classList.remove('show');
        });
    });

    // -------------------------------
    // Klick utanför modalkortet = stäng
    // -------------------------------
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', e => {
            if (e.target === modal) modal.classList.remove('show');
        });
    });

    // -------------------------------
    // .btn-actions för varje projektkort (via delegation)
    // -------------------------------
    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-actions');
        if (btn) {
            e.stopPropagation();
            const card = btn.closest('.project.card');
            const dropdown = card.querySelector('.project-dropdown');

            document.querySelectorAll('.project-dropdown').forEach(d => {
                if (d !== dropdown) d.classList.remove('show');
            });

            if (dropdown) dropdown.classList.toggle('show');
        }
    });

   
    // ---------------------------------------------
    // RADERA PROJEKT
    // ---------------------------------------------
    document.addEventListener('click', async (e) => {
        const deleteBtn = e.target.closest('.delete-btn');
        if (deleteBtn) {
            const projectId = deleteBtn.getAttribute('data-id');

            if (confirm("Are you sure you want to delete this project?")) {
                const response = await fetch('/admin/projects/delete', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ id: projectId })
                });

                const result = await response.json();

                if (result.success) {
                    // Ta bort projektkortet från DOM
                    const projectCard = deleteBtn.closest('.project.card');
                    if (projectCard) {
                        projectCard.remove();
                    }
                } else {
                    alert('Error deleting project: ' + (result.error || 'Unknown error'));
                }
            }
        }
    });

    // ---------------------------------------------
    // HÄMTA OCH FYLLA EDIT-MODALEN
    // ---------------------------------------------

    document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener("click", async () => {
            const projectId = btn.getAttribute("data-id");

            const res = await fetch(`/admin/projects/api/project/${projectId}`);
            const data = await res.json();

            // Fyll formulärfält
            document.querySelector("#edit-project-modal input[name='Id']").value = data.project.id;
            document.querySelector("#edit-project-modal input[name='ProjectName']").value = data.project.projectName;
            document.querySelector("#edit-project-modal textarea[name='Description']").value = data.project.description || "";
            document.querySelector("#edit-project-modal input[name='StartDate']").value = data.project.startDate?.split("T")[0] || "";
            document.querySelector("#edit-project-modal input[name='EndDate']").value = data.project.endDate?.split("T")[0] || "";
            document.querySelector("#edit-project-modal input[name='Budget']").value = data.project.budget ?? "";
            document.querySelector("#edit-project-modal select[name='StatusId']").value = data.project.statusId;

            const quill = Quill.find(document.querySelector("[data-quill-editor='edit-project-description-wysiwyg-editor']"));
            if (quill) {
                quill.root.innerHTML = data.project.description || "";
                document.getElementById("edit-project-description").value = data.project.description || "";
            }

            // Fyll klient-dropdown
            const clientNameInput = document.querySelector("#edit-project-modal input[name='ClientName']");
            if (clientNameInput) {
                clientNameInput.value = data.project.clientName ?? "";
            }


            // Visa modalen
            document.querySelector("#edit-project-modal").classList.add("show");
        });
    });

    // ---------------------------------------------
    // ADD PROJEKT VALIDATION
    // ---------------------------------------------
    function validateAddProjectForm() {
        const form = document.getElementById("add-project-form");
        if (!form) return false;

        let isValid = true;

        const projectName = form.querySelector('[name="ProjectName"]');
        const clientName = form.querySelector('[name="ClientName"]');
        const description = document.getElementById("add-project-description"); // ✅ fortfarande referens, men ingen validering
        const startDate = form.querySelector('[name="StartDate"]');
        const endDate = form.querySelector('[name="EndDate"]');
        const budget = form.querySelector('[name="Budget"]');
        const status = form.querySelector('[name="StatusId"]');

        form.querySelectorAll(".form-error").forEach(el => el.remove());
        form.querySelectorAll(".is-invalid").forEach(el => el.classList.remove("is-invalid"));

        function showError(input, message) {
            const error = document.createElement("div");
            error.className = "form-error";
            error.textContent = message;

            const group = input.closest(".form-group");
            if (group) {
                group.appendChild(error); // 🔧 Placera inuti .form-group, inte efter
            } else {
                input.insertAdjacentElement("afterend", error);
            }

            input.classList.add("is-invalid");
            isValid = false;
        }



        if (!projectName.value.trim()) {
            showError(projectName, "Project name is required");
        }

        if (!clientName.value.trim()) {
            showError(clientName, "Client name is required");
        }

        // 🟡 description-fältet är inte längre required

        if (!startDate.value) {
            showError(startDate, "Start date is required");
        }

        if (!endDate.value) {
            showError(endDate, "End date is required");
        } else if (startDate.value && new Date(endDate.value) < new Date(startDate.value)) {
            showError(endDate, "End date must be after start date");
        }

        if (!budget.value.trim()) {
            showError(budget, "Budget is required");
        }

        if (!status.value || (status.value !== "1" && status.value !== "2")) {
            showError(status, "Please select a valid status");
        }

        return isValid;
    }


    // ---------------------------------------------
    // FORMULÄRINLÄMNING: Skicka POST och redirecta (ADD)
    // ---------------------------------------------
    const addProjectForm = document.getElementById("add-project-form");
    if (addProjectForm) {
        addProjectForm.addEventListener("submit", async (e) => {
            e.preventDefault(); // ⛔ alltid stoppa först

            // ✅ Kör JS-validering innan något annat
            if (!validateAddProjectForm()) return;

            // 🔄 Synka Quill-data till textarea
            const textarea = document.getElementById("add-project-description");
            const editor = document.querySelector("[data-quill-editor='add-project-description-wysiwyg-editor']");
            if (editor && textarea) {
                const quill = Quill.find(editor);
                if (quill) {
                    textarea.value = quill.root.innerHTML;
                }
            }

            const formData = new FormData(addProjectForm);

            const response = await fetch("/admin/projects/add", {
                method: "POST",
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                window.location.href = "/admin/projects";
            } else {
                const errorContainer = document.createElement("div");
                errorContainer.className = "form-error text-danger mt-2";
                errorContainer.textContent = result.error ?? "Something went wrong. Please check your input.";

                const formTop = addProjectForm.querySelector(".card-body");
                if (formTop && !formTop.querySelector(".form-error")) {
                    formTop.prepend(errorContainer);
                }
            }
        });
    }


    // ---------------------------------------------
    // FORMULÄRINLÄMNING: Skicka POST och redirecta (EDIT)
    // ---------------------------------------------
    const editProjectForm = document.getElementById("edit-project-form");
    if (editProjectForm) {
        editProjectForm.addEventListener("submit", async (e) => {
            e.preventDefault();

            const textarea = document.getElementById("edit-project-description");
            const editor = document.querySelector("[data-quill-editor='edit-project-description-wysiwyg-editor']");
            if (editor && textarea) {
                const quill = Quill.find(editor);
                if (quill) {
                    textarea.value = quill.root.innerHTML;
                }
            }

            const formData = new FormData(editProjectForm);

            const response = await fetch("/admin/projects/update", {
                method: "POST",
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                window.location.reload(); // 🔄 Uppdatera sidan så man ser förändringen direkt
            } else {
                alert("Kunde inte uppdatera projekt: " + (result.error ?? "Okänt fel."));
            }
        });
    }

    // ---------------------------------------------
    // FILTRERING AV PROJEKTKORT
    // ---------------------------------------------

    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            const status = btn.getAttribute('data-status');

            document.querySelectorAll('.project.card').forEach(card => {
                const cardStatus = card.getAttribute('data-status');

                if (status === 'all' || cardStatus === status) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });

    // ---------------------------------------------
    // TRIMMA FÖRHANDSBESKRIVNINGAR PÅ CARDS
    // ---------------------------------------------
    document.querySelectorAll('.project-description').forEach(el => {
        const rawHtml = el.innerHTML;

        const tempDiv = document.createElement("div");
        tempDiv.innerHTML = rawHtml;
        const plainText = tempDiv.textContent || tempDiv.innerText || "";

        const charLimit = 120;
        const truncated = plainText.length > charLimit
            ? plainText.slice(0, charLimit).trim() + "…"
            : plainText.trim();

        el.textContent = truncated;
    });


    // ---------------------------------------------
    // SIGNIN VALIDATION
    // ---------------------------------------------
    window.addEventListener("DOMContentLoaded", function () {
        const form = document.getElementById("signin-form");
        console.log("🔍 Form hittad?", form);

        if (!form) return;

        const email = document.getElementById("email");
        const password = document.getElementById("password");

        form.addEventListener("submit", function (e) {
            console.log("🚀 Submit-triggad");

            let isValid = true;

            form.querySelectorAll(".form-error").forEach(el => el.remove());
            form.querySelectorAll(".is-invalid").forEach(el => el.classList.remove("is-invalid"));

            function showError(input, message) {
                console.log(`❌ Fel på: ${input.name} → ${message}`);
                const error = document.createElement("div");
                error.className = "form-error";
                error.textContent = message;
                input.classList.add("is-invalid");
                input.insertAdjacentElement("afterend", error);
                isValid = false;
            }

            if (!email.value.trim()) {
                showError(email, "Email is required");
            } else if (!/^[\w-.]+@([\w-]+\.)+[\w-]{2,}$/.test(email.value)) {
                showError(email, "Enter a valid email");
            }

            if (!password.value.trim()) {
                showError(password, "Password is required");
            }

            if (!isValid) {
                e.preventDefault();
                console.log("Formulär EJ skickat pga valideringsfel");
            } else {
                console.log("Form OK – skickas");
            }
        });
    });
    // ---------------------------------------------
    // SIGNUP VALIDATION
    // ---------------------------------------------
    window.addEventListener("DOMContentLoaded", function () {
        const form = document.getElementById("signup-form");
        if (!form) return;

        const fullName = document.getElementById("fullname");
        const email = document.getElementById("email");
        const password = document.getElementById("password");
        const confirmPassword = document.getElementById("confirmPassword");
        const terms = document.getElementById("terms");
        const termsErrorContainer = document.getElementById("terms-error-container");

        form.addEventListener("submit", function (e) {
            let isValid = true;

            // Rensa tidigare fel
            form.querySelectorAll(".form-error").forEach(el => el.remove());
            form.querySelectorAll(".is-invalid").forEach(el => el.classList.remove("is-invalid"));
            if (termsErrorContainer) termsErrorContainer.innerHTML = "";

            function showError(input, message) {
                const error = document.createElement("div");
                error.className = "form-error";
                error.textContent = message;
                input.classList.add("is-invalid");
                input.insertAdjacentElement("afterend", error);
                isValid = false;
            }

            // Full name
            if (!fullName.value.trim()) {
                showError(fullName, "Full name is required");
            } else if (!/^[a-zA-ZåäöÅÄÖ\s\-']+$/.test(fullName.value)) {
                showError(fullName, "Only letters, spaces, hyphens and apostrophes are allowed");
            }

            // Email
            if (!email.value.trim()) {
                showError(email, "Email is required");
            } else if (!/^[\w-.]+@([\w-]+\.)+[\w-]{2,}$/.test(email.value)) {
                showError(email, "Enter a valid email address");
            }

            // Password
            if (!password.value.trim()) {
                showError(password, "Password is required");
            } else if (password.value.length < 6) {
                showError(password, "Password must be at least 6 characters");
            } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(password.value)) {
                showError(password, "Password must include uppercase, lowercase and a number");
            }


            // Confirm Password
            if (!confirmPassword.value.trim()) {
                showError(confirmPassword, "Please confirm your password");
            } else if (confirmPassword.value !== password.value) {
                showError(confirmPassword, "Passwords do not match");
            }

            // Terms checkbox – hanteras separat
            if (!terms.checked && termsErrorContainer) {
                const error = document.createElement("div");
                error.className = "form-error";
                error.textContent = "You must accept the terms";
                termsErrorContainer.appendChild(error);
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault(); // Stop form submission
            }
        });
    });


    










});
