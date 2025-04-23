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
});
