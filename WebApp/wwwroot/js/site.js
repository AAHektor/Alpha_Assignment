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
            const clientSelect = document.querySelector("#edit-project-modal select[name='ClientId']");
            clientSelect.innerHTML = '<option value="">-- Select Client --</option>';
            data.clients.forEach(client => {
                const option = document.createElement("option");
                option.value = client.id;
                option.textContent = client.name;
                if (client.id === data.project.clientId)
                    option.selected = true;
                clientSelect.appendChild(option);
            });

            // Visa modalen
            document.querySelector("#edit-project-modal").classList.add("show");
        });
    });

    // ---------------------------------------------
    // FORMULÄRINLÄMNING: Skicka POST och redirecta (ADD)
    // ---------------------------------------------
    const addProjectForm = document.getElementById("add-project-form");
    if (addProjectForm) {
        addProjectForm.addEventListener("submit", async (e) => {
            e.preventDefault();

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
                alert("Kunde inte skapa projekt: " + (result.error ?? "Okänt fel."));
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



});
