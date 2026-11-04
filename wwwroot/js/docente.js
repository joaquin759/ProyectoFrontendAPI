
const API_DOCENTE_URL = "/api/Docente";

const formulario = document.getElementById("formDocente");
const tabla = document.getElementById("tablaDocentes");
const mensaje = document.getElementById("mensaje");

// Cargar docentes al iniciar
document.addEventListener("DOMContentLoaded", cargarDocente);

// Consultar docentes
async function cargarDocente() {
    try {
        const respuesta = await fetch(`${API_DOCENTE_URL}/Lista`);

        if (!respuesta.ok) {
            throw new Error("No se pudieron cargar los registros.");
        }

        const docentes = await respuesta.json();
        tabla.innerHTML = "";

        if (docentes.length === 0) {
            tabla.innerHTML = `
                <tr><td colspan="8">No hay registros disponibles.</td></tr>
            `;
            return;
        }

        docentes.forEach(docente => {
            const fila = document.createElement("tr");

            const valores = [
                docente.idDocente,
                docente.nombreCompleto,
                docente.correo,
                docente.tipo,
                docente.horasContratadas ?? "-",
                docente.disponibilidad || "-",
                docente.observaciones || "-"
            ];

            valores.forEach(valor => {
                const celda = document.createElement("td");
                celda.textContent = valor;
                fila.appendChild(celda);
            });

            const acciones = document.createElement("td");

            const btnEditar = document.createElement("button");
            btnEditar.textContent = "Editar";
            btnEditar.type = "button";
            btnEditar.addEventListener("click", () =>
                editarDocente(docente.idDocente)
            );

            const btnEliminar = document.createElement("button");
            btnEliminar.textContent = "Eliminar";
            btnEliminar.type = "button";
            btnEliminar.className = "secondary";
            btnEliminar.addEventListener("click", () =>
                eliminarDocente(docente.idDocente)
            );

            acciones.append(btnEditar, btnEliminar);
            fila.appendChild(acciones);
            tabla.appendChild(fila);
        });

    } catch (error) {
        tabla.innerHTML = `
            <tr><td colspan="8">Error al cargar los registros.</td></tr>
        `;
        mostrarMensaje(error.message, true);
    }
}

// Validar datos del docente
function validarDocente(docente) {

    if (!docente.nombreCompleto || !docente.correo || !docente.tipo) {
        return "Completa el nombre, correo y tipo de docente.";
    }

    if (docente.nombreCompleto.length < 3) {
        return "El nombre debe tener al menos 3 caracteres.";
    }

    if (/\d/.test(docente.nombreCompleto)) {
        return "El nombre no debe contener números.";
    }

    const correoValido = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!correoValido.test(docente.correo)) {
        return "Ingresa un correo electrónico válido.";
    }

    if (!["PA", "PTC"].includes(docente.tipo)) {
        return "Selecciona un tipo de docente válido.";
    }

    if (
        docente.horasContratadas !== null &&
        (!Number.isFinite(docente.horasContratadas) ||
            docente.horasContratadas <= 0 ||
            docente.horasContratadas > 40)
    ) {
        return "Las horas deben ser mayores a 0 y no exceder 40.";
    }

    return null;
}

// Registrar o actualizar docente
formulario.addEventListener("submit", async (e) => {
    e.preventDefault();

    const id = document.getElementById("idDocente").value;

    const docente = {
        nombreCompleto: document.getElementById("nombreCompleto").value.trim(),
        correo: document.getElementById("correo").value.trim(),
        tipo: document.getElementById("tipo").value,
        horasContratadas: document.getElementById("horasContratadas").value
            ? Number(document.getElementById("horasContratadas").value)
            : null,
        disponibilidad: document.getElementById("disponibilidad").value.trim() || null,
        observaciones: document.getElementById("observaciones").value.trim() || null
    };

    if (id) docente.idDocente = Number(id);

    // Ejecutar validaciones antes de enviar a la API
    const errorValidacion = validarDocente(docente);

    if (errorValidacion) {
        mostrarMensaje(errorValidacion, true);
        return;
    }

    try {
        const respuesta = await fetch(API_DOCENTE_URL, {
            method: id ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(docente)
        });

        const resultado = await respuesta.json();

        if (!respuesta.ok || !resultado.isSuccess) {
            throw new Error(
                resultado.respuesta || "No se pudo guardar el registro."
            );
        }

        mostrarMensaje(resultado.respuesta);
        limpiarFormulario();
        await cargarDocente();

    } catch (error) {
        mostrarMensaje(error.message, true);
    }
});

// Consultar docente para editar
async function editarDocente(id) {
    try {
        const respuesta = await fetch(`${API_DOCENTE_URL}/${id}`);

        if (!respuesta.ok) {
            throw new Error("No se encontró el registro.");
        }

        const docente = await respuesta.json();

        document.getElementById("idDocente").value = docente.idDocente;
        document.getElementById("nombreCompleto").value = docente.nombreCompleto;
        document.getElementById("correo").value = docente.correo;
        document.getElementById("tipo").value = docente.tipo;
        document.getElementById("horasContratadas").value =
            docente.horasContratadas ?? "";
        document.getElementById("disponibilidad").value =
            docente.disponibilidad ?? "";
        document.getElementById("observaciones").value =
            docente.observaciones ?? "";

        document.getElementById("tituloFormulario").textContent = "Editar docente";
        document.getElementById("btnGuardar").textContent = "Guardar cambios";
        document.getElementById("btnCancelar").hidden = false;

        window.scrollTo({ top: 0, behavior: "smooth" });

    } catch (error) {
        mostrarMensaje(error.message, true);
    }
}

// Eliminar docente
async function eliminarDocente(id) {
    if (!confirm("¿Deseas eliminar este registro?")) return;

    try {
        const respuesta = await fetch(`${API_DOCENTE_URL}/${id}`, {
            method: "DELETE"
        });

        const resultado = await respuesta.json();

        if (!respuesta.ok || !resultado.isSuccess) {
            throw new Error(resultado.respuesta || "No se pudo eliminar.");
        }

        mostrarMensaje(resultado.respuesta);
        await cargarDocente();

    } catch (error) {
        mostrarMensaje(error.message, true);
    }
}

// Limpiar formulario
function limpiarFormulario() {
    formulario.reset();

    document.getElementById("idDocente").value = "";
    document.getElementById("tituloFormulario").textContent = "Registrar docente";
    document.getElementById("btnGuardar").textContent = "Guardar docente";
    document.getElementById("btnCancelar").hidden = true;
}

// Mostrar mensajes
function mostrarMensaje(texto, error = false) {
    mensaje.textContent = texto;
    mensaje.style.color = error ? "red" : "green";
}

// Botones adicionales
document.getElementById("btnCancelar").addEventListener("click", limpiarFormulario);
document.getElementById("btnActualizar").addEventListener("click", cargarDocente);
