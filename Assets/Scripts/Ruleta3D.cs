using UnityEngine;
using UnityEngine.InputSystem;

public class Ruleta3D : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private float sensitivity = 1f;

    [SerializeField]
    private bool invertir = false;

    [SerializeField]
    private float ruletaMin = 10f;

    [SerializeField]
    private float ruletaMax = 200f;

    [SerializeField]
    private float distanciaMinimaCentroPx = 20f;

    private bool estaDragging = false;
    private Camera camPrincipal;
    private float xBase;
    private float xRelativoActual;
    private float desfaseArrastre;
    private float anguloMouseAnterior;
    private float anguloMouseContinuo;
    private float yBloqueada;
    private float zBloqueada;

    private Mouse mouse;

    void Start()
    {
        camPrincipal = Camera.main;
        mouse = Mouse.current;

        Vector3 rotacionInicial = transform.localEulerAngles;
        xBase = NormalizarAngulo(rotacionInicial.x);
        yBloqueada = rotacionInicial.y;
        zBloqueada = rotacionInicial.z;

        AplicarLimitesRotacion();
    }

    void Update()
    {
        if (mouse == null)
        {
            return;
        }

        // Detectar pulsación del botón izquierdo
        if (mouse.leftButton.wasPressedThisFrame)
        {
            IniciarDrag();
        }

        if (mouse.leftButton.isPressed && estaDragging)
        {
            ActualizarRotacion();
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            estaDragging = false;
        }
    }

    private void IniciarDrag()
    {
        Vector2 posRaton = mouse.position.ReadValue();
        if (!TryObtenerAngulo(posRaton, out float anguloActual))
        {
            estaDragging = false;
            return;
        }

        float factorDireccion = ObtenerFactorDireccion();

        anguloMouseAnterior = anguloActual;
        anguloMouseContinuo = anguloActual;

        // Mantiene continuidad al empezar a arrastrar y evita saltos de -180/180.
        desfaseArrastre = xRelativoActual - (anguloMouseContinuo * sensitivity * factorDireccion);
        estaDragging = true;
    }

    private void ActualizarRotacion()
    {
        Vector2 posRaton = mouse.position.ReadValue();
        if (!TryObtenerAngulo(posRaton, out float anguloActual))
        {
            return;
        }

        float deltaMouse = Mathf.DeltaAngle(anguloMouseAnterior, anguloActual);
        anguloMouseContinuo += deltaMouse;
        anguloMouseAnterior = anguloActual;

        float factorDireccion = ObtenerFactorDireccion();

        float limiteMin = Mathf.Min(ruletaMin, ruletaMax);
        float limiteMax = Mathf.Max(ruletaMin, ruletaMax);
        float xObjetivoRelativo =
            (anguloMouseContinuo * sensitivity * factorDireccion) + desfaseArrastre;

        xRelativoActual = Mathf.Clamp(xObjetivoRelativo, limiteMin, limiteMax);
        float nuevoX = xBase + xRelativoActual;
        transform.localEulerAngles = new Vector3(nuevoX, yBloqueada, zBloqueada);
    }

    private void AplicarLimitesRotacion()
    {
        float limiteMin = Mathf.Min(ruletaMin, ruletaMax);
        float limiteMax = Mathf.Max(ruletaMin, ruletaMax);

        float xActualNormalizada = NormalizarAngulo(transform.localEulerAngles.x);
        xRelativoActual = Mathf.DeltaAngle(xBase, xActualNormalizada);
        xRelativoActual = Mathf.Clamp(xRelativoActual, limiteMin, limiteMax);

        float xFinal = xBase + xRelativoActual;
        transform.localEulerAngles = new Vector3(xFinal, yBloqueada, zBloqueada);
    }

    /// <summary>
    /// Calcula el ángulo del ratón respecto al centro del objeto en pantalla.
    /// </summary>
    private float ObtenerAngulo(Vector2 posRaton)
    {
        // Convertir posición del objeto a coordenadas de pantalla
        Vector2 centroEnPantalla = camPrincipal.WorldToScreenPoint(transform.position);

        Vector2 direccion = posRaton - centroEnPantalla;
        return Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
    }

    private bool TryObtenerAngulo(Vector2 posRaton, out float angulo)
    {
        Vector2 centroEnPantalla = camPrincipal.WorldToScreenPoint(transform.position);
        Vector2 direccion = posRaton - centroEnPantalla;

        if (direccion.sqrMagnitude < (distanciaMinimaCentroPx * distanciaMinimaCentroPx))
        {
            angulo = 0f;
            return false;
        }

        angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        return true;
    }

    private float NormalizarAngulo(float angulo)
    {
        return Mathf.Repeat(angulo + 180f, 360f) - 180f;
    }

    private float ObtenerFactorDireccion()
    {
        // El signo base corrige el sentido para que el arrastre se sienta natural.
        return invertir ? 1f : -1f;
    }
}
