$(document).ready(function () {
    let editarIndex = -1; // Variable para saber si estamos editando un pedido

    // Al enviar el formulario de pedido (Agregar o editar)
    $('#pedidoForm').submit(function (event) {
        event.preventDefault();  // Evita el envío tradicional del formulario

        // Obtén los valores de los campos
        var nombreCliente = $('#clienteNombre').val();
        var mesa = $('#clienteMesa').val();
        var producto = $('#producto').val();
        var cantidad = $('#cantidad').val();
        var tipoPago = $('#tipoPago').val();

        // Verifica si se han completado todos los campos
        if (nombreCliente && mesa && producto && cantidad && tipoPago) {
            var personalizacion = [];
            if ($('#extraAzucar').is(':checked')) {
                personalizacion.push('Extra Azúcar');
            }
            if ($('#extraLeche').is(':checked')) {
                personalizacion.push('Extra Leche');
            }

            // Crear fila con el pedido
            var filaPedido = `
                <tr>
                    <td>${producto}</td>
                    <td>${cantidad}</td>
                    <td>${personalizacion.join(', ') || 'Ninguna'}</td>
                    <td>${tipoPago}</td>
                    <td>
                        <button type="button" class="btn btn-warning btn-sm editarPedido">Editar</button>
                        <button type="button" class="btn btn-danger btn-sm eliminarPedido">Eliminar</button>
                    </td>
                </tr>
            `;

            if (editarIndex === -1) {
                // Si no estamos editando, agregar un nuevo pedido
                $('#pedidosList').append(filaPedido);
            } else {
                // Si estamos editando, actualizar el pedido en la tabla
                $('#pedidosList tr').eq(editarIndex).html(filaPedido);
            }

            // Limpiar los campos del formulario
            $('#pedidoForm')[0].reset();
            $('#submitButton').text('Agregar Pedido'); // Cambiar el texto del botón
            editarIndex = -1; // Resetear la variable de edición
        } else {
            alert('Por favor, complete todos los campos.');
        }
    });

    // Eliminar pedido
    $(document).on('click', '.eliminarPedido', function () {
        $(this).closest('tr').remove();
    });

    // Editar pedido
    $(document).on('click', '.editarPedido', function () {
        // Obtener la fila del pedido que estamos editando
        var fila = $(this).closest('tr');

        // Rellenar los campos del formulario con los valores del pedido
        $('#clienteNombre').val(fila.find('td').eq(0).text()); // Nombre del cliente
        $('#clienteMesa').val(fila.find('td').eq(1).text()); // Mesa
        $('#producto').val(fila.find('td').eq(2).text()); // Producto
        $('#cantidad').val(fila.find('td').eq(3).text()); // Cantidad
        $('#tipoPago').val(fila.find('td').eq(4).text()); // Tipo de pago

        // Marcar las opciones de personalización
        var personalizacion = fila.find('td').eq(2).text();
        $('#extraAzucar').prop('checked', personalizacion.includes('Extra Azúcar'));
        $('#extraLeche').prop('checked', personalizacion.includes('Extra Leche'));

        // Cambiar el texto del botón a "Actualizar Pedido"
        $('#submitButton').text('Actualizar Pedido');

        // Establecer el índice de la fila que estamos editando
        editarIndex = fila.index();
    });
});
