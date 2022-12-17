# TP4_Plataformas_de_Desarrollo

![image](https://user-images.githubusercontent.com/48494284/208215318-9aa70f62-2454-47fa-9732-ed8d3ecf7d65.png)

<h1>Trabajo:</h1>
<p>El objetivo de este trabajo es desarrollar una web, donde implementaremos el modelo de negocio el cual trabajamos durante todo el cuatrimestre, utilizando MVC Framework Core junto con ORM para la persistencia de datos. </p>
<p>Al iniciar nuestra Web, la primera ventana a visualizar es la de “Inicia Sesión para continuar”. Para poder ingresar a la web, en la cabecera se encontrará un Item de “Iniciar sesión” que llevará al Login de esta página, donde únicamente se requiere el DNI Y Contraseña del usuario. Si este mismo no está logueado, se le brinda al costado del Botón de “iniciar”, la opción de registrarse. Al hacer click en registrarse, el programa solicita que se llenen todos los campos, de no ser así, se le presentara al usuario mensajes de error para que complete de forma correcta los datos requeridos. Las condiciones para que el registro sea exitoso son que principalmente que todos los campos estén completos, el DNI de tipo numérico, tenga un límite de 7 dígitos y que el mismo no este previamente registrado en el sistema, que el mail ingresado conserve el formato correcto de tipo “email”. En la pagina del Login, el sistema tras surgir un error en el DNI ingresado, este muestra un mensaje de que no pertenece a ningún usuario, si el usuario comete equivocaciones en el relleno de campos, el sistema expone los intentos restantes que le quedan antes de ser bloqueado y por consecuencia cuando el mismo llega al limite de 3 intentos fallidos se bloquea definitivamente al usuario y no podrá volver a intentar entrar. A continuación, si se inicia sesión correctamente, se presentará la web con un total de 4 pestañas, las mismas son: Cajas de ahorro, Plazos fijos, Pagos y Tarjetas. En Cajas de ahorro, se visualiza una lista de cajas de ahorro que se rellenara mediante hagamos click en el botón “Nueva caja de ahorro”, que nos crea un CBU nuevo, nos muestra el saldo que tenemos actualmente que inicia en cero y el titular que decide crearla. Por otro lado, al seleccionar una caja de la lista se se encontrarán al costado derecho los botones que contienen las acciones que pueden realizarse como “Ver movimientos” donde se ve nuestro CBU, saldo, nombre de titular y movimiento que se realizó. La acción “Agregar titular” que la condición que se debe cumplir es la de ya estar registrado en el sistema y en la ventana emergente que se expone, se pedirá ingresar DNI con el objetivo de identificar al usuario y agregarlo a la caja de ahorro. “Eliminar titular”, que al igual que “Agregar titular”, debe cumplir la condición de estar registrado en el sistema y despliega una ventana emergente que también solicita poder identificar al usuario mediante el DNI y eliminarlo de la caja de ahorro. “Depositar”, donde se verán datos como CBU, saldo y el titular, más una ventana emergente que se despliega para cumplir la función de colocar el monto a depositar y si el usuario deposita, el saldo se incrementa. “Retirar”, expone los datos CBU, saldo y titulares, también se presenta una ventana donde se realizará la acción de colocar que monto deseo retirar. “Transferir”, donde tenemos los datos CBU, saldo, titulares, al igual que las demás acciones, y dos ventanas, en las cuales, en una de ellas se permite dejar el monto a depositar y en la otra, para saber hacia dónde transferir, el CBU del destinatario. Por último, la acción “Eliminar” es la que definitivamente eliminara la Caja de ahorro si esto se desea, se presenta el CBU y el saldo que tiene el usuario, pero la condición para eliminarla es que no contenga saldo, caso contrario, no la eliminara y me mostrara un mensaje “Tiene saldo, no se puede eliminar.” Si la caja no tiene saldo, se eliminará exitosamente y no se visualizará más en la lista de cajas de ahorro. Es importante aclarar que cada acción, si oprime, tendrá la opción de “Volver” que me redirigirá al index nuevamente. En la pestaña de “Pagos”, se muestra la lista de pagos, con los datos del nombre del pago que se realizó, monto, y método del mismo. Si se accede al botón “Crear nuevo pago”, el cual me permite agregar uno nuevo, se mostrarán 2 campos, uno que permite colocar el nombre del pago y otro que es el monto a pagar, por último, este se podrá crear si se rellenan todos los campos solicitados por el Sistema. Una vez creado el pago, me redirigirá nuevamente al Index y para terminar esta acción, se visualizarán los botones: “Pagar con caja de ahorro” donde se solicita seleccionar la caja de ahorro. Si la caja no tiene saldo suficiente, me lo aclarara con un mensaje “La caja tiene saldo insuficiente “ y si esta si tiene saldo suficiente, terminara colocando en la lista de pagos el pago realizado con éxito. Por otro lado, “Pagar con tarjeta”, pedirá que se seleccione la tarjeta previamente creada para realizar el pago y si no se selecciona ninguna, se expone un mensaje de error: “Seleccione una tarjeta”. Si luego de seleccionar la tarjeta, el sistema la toma, se podrá terminar realizando la acción de “Pagar”. En la pestaña de “Tarjetas”, principalmente se muestra una lista de tarjetas con los datos del numero de la tarjeta, el límite de 20000, y consumo. En esta pestaña, se presenta la oportunidad de crear la tarjeta con el botón “Nueva Tarjeta” donde luego de presionarlo, ahora si se visualizara por completo el número de tarjeta generado, el límite y consumo que inicia en cero si aun no se realizó ninguno. Una vez creada la tarjeta, se muestra un botón de “Eliminar” que te dirige a su interfaz donde se ve el numero de tarjeta, el límite de 20000, consumo y titular, finalmente, la misma se eliminara, dirigiendo al usuario al index, mostrándole que ya no existe mas la tarjeta dentro de la lista. En la pestaña de “Plazos fijos”, principalmente se muestra la lista de plazos fijos con el monto, la fecha de finalización y el cbu. Dentro de la pestaña hay un botón de “Nuevo plazo fijo” que dirige a crearlo con la condición de rellenar el monto y seleccionar la caja de ahorro que se utilizara. Si el monto es insuficiente, no se podrá crear el plazo fijo, pero si este tiene el saldo para poder crearlo, la acción será exitosa y redirigirá al Index para mostrar que el plazo fijo ya esta creado. Para finalizar, si el usuario desea cerrar sesión (item colocado en a cabecera de la página), este me enviara a la pagina inicial donde principalmente se abrió la página.</p>
<p>El administrador, papel importante dentro de nuestro trabajo ya que hemos establecido condiciones para que tenga acceso a datos que el usuario no puede manipular, es aquel que tendrá en el menú la pestaña de Usuarios para poder realizar las acciones: “Editar”, la cual permitirá al usuario administrador editar el DNI, nombre, apellido, mail, intentos fallidos y si el usuario será administrador o no. Como condición, el usuario debe ser únicamente administrador y rellenar los usuarios de manera correcta, debido a que si no se completan de manera adecuada, se expondrá un mensaje de error. La acción “Detalles” que cumple la función de mostrar los detalles del usuario y si el usuario administrador, requiere editarlos se lo redirigirá a la pestaña de “Editar”. La acción “Bloquear”, es aquella que permite bloquear al usuario si este existe y si no, expone un mensaje de error “Error al buscar usuario con el “id” en la base de datos”. Es importante aclarar que si el usuario es definitivamente bloqueado por decisión del administrador, este no podrá acceder y se le mostrara un mensaje de “Usuario bloqueado” cuando intente iniciar sesion. La acción “Desbloquear”, que permite desbloquear al usuario si el administrador lo desea y por consiguiente el usuario podrá tener la posibilidad de loguearse efectivamente. Por último, la acción “Eliminar”, botón que dirige hacia su pestaña donde se llevara a cabo la acción y esta muestra el DNI, nombre, apellido, mail, intentos fallidos, si el usuario se encuentra bloqueado o no y si el mismo es administrador. Una vez que se ingresa a esta pestaña se podrá eliminar al usuario y redirigirá a la pestaña de “Usuarios” en donde se expone que el usuario ha sido eliminado. Es de suma importancia, volver a reiterar que cada acción tendrá su opción de “Volver”, que al presionarla nos enviara al Index nuevamente.</p>
