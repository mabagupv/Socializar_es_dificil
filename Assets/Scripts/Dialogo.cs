using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogo : MonoBehaviour
{
    [SerializeField]
    private NPCDialogueSO[] dialogueData;

    [SerializeField]
    private EmotionType emotion; // ← se asigna desde el Inspector

    TextMeshProUGUI objDialogo;
    public string frase = "";

    [SerializeField]
    float velocidadEscribir = 0.05f;
    float tiempoEntreFrases = 2.0f;
    private List<int> bloquesDisponibles = new List<int>();
    int bloqueIdentificador = 0;
    bool escribiendoBloque = false;
    bool esperandoContinuacion = false;
    int minListaBloques = 1;
    int maxListaBloques = 2;

    public float exito = 0.5f;

    void Start()
    {
        objDialogo = this.GetComponent<TextMeshProUGUI>();
        objDialogo.text = " ";

        for (int i = minListaBloques; i < maxListaBloques; i++)
        {
            bloquesDisponibles.Add(i);
        }
    }

    void Update()
    {
        Debug.Log(bloqueIdentificador);
        if (escribiendoBloque == false)
        {
            if (bloqueIdentificador == 0)
            {
                // StartCoroutine(Intro());
            }
            else if (bloqueIdentificador == 1)
            {
                // StartCoroutine(Bloque1());
            }
        }
    }

    public void IniciarDialogo()
    {
        if (escribiendoBloque)
        {
            return;
        }

        StartCoroutine(PlayDialogues(dialogueData));
    }

    private IEnumerator PlayDialogues(NPCDialogueSO[] dialogues)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            yield break;
        }

        escribiendoBloque = true;

        for (int i = 0; i < dialogues.Length; i++)
        {
            NPCDialogueSO dialogue = dialogues[i];

            if (dialogue == null)
            {
                continue;
            }

            yield return StartCoroutine(PlayDialogue(dialogue));

            if (HasPendingDialogue(dialogues, i + 1))
            {
                esperandoContinuacion = true;
                yield return new WaitUntil(() => esperandoContinuacion == false);
            }
        }

        escribiendoBloque = false;
    }

    private bool HasPendingDialogue(NPCDialogueSO[] dialogues, int startIndex)
    {
        for (int i = startIndex; i < dialogues.Length; i++)
        {
            if (dialogues[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    //REPRODUCE EL DIALOGO LETRA POR LETRA
    private IEnumerator PlayDialogue(NPCDialogueSO dialogue)
    {
        if (dialogue.blocks == null)
        {
            yield break;
        }

        EmotionType currentEmotion = emotion;

        // para cada bloque de diálogo, dependiendo de su tipo, se asigna la frase a mostrar y se modifica el éxito según la emoción actual
        foreach (DialogueBlock block in dialogue.blocks)
        {
            Debug.Log($"Procesando bloque: {block.blockType}");

            //si es fijo
            if (block.blockType == DialogueBlockType.Fixed)
            {
                frase = block.fixedPhrase;
            }
            //en caso contrario, si es emocional
            else if (block.blockType == DialogueBlockType.Emotional)
            {
                if (block.TryGetEmotionalResponse(currentEmotion, out EmotionalResponse response))
                {
                    frase = response.phrase;
                    exito += response.successModifier;
                }
            }

            yield return StartCoroutine(EscribirLento());
        }
    }

    //ESCRIBE "FRASE" EN PANTALLA LETRA POR LETRA, ESPERA tiempoEntreFrases SEGUNDOS Y BORRA EL TEXTO.
    IEnumerator EscribirLento()
    {
        foreach (char c in frase)
        {
            objDialogo.text += c;
            yield return new WaitForSeconds(velocidadEscribir);
        }
        yield return new WaitForSeconds(tiempoEntreFrases);
        objDialogo.text = " ";
    }

    public void ContinuarDialogo()
    {


        if (!escribiendoBloque || !esperandoContinuacion)
        {
            return;
        }

        esperandoContinuacion = false;
    }

    //Para pruebas: lo vinculo desde los botones del UI
    public void AplicarEmocion(string newEmotion)
    {
        if (newEmotion == "neutral")
        {
            emotion = EmotionType.Neutral;
        }
        else if (newEmotion == "feliz")
        {
            emotion = EmotionType.Feliz;
        }
        else if (newEmotion == "enfadado")
        {
            emotion = EmotionType.Enfadado;
        }
        else if (newEmotion == "triste")
        {
            emotion = EmotionType.Triste;
        }
        else if (newEmotion == "sorprendido")
        {
            emotion = EmotionType.Sorprendido;
        }
    }
}


/*

    IEnumerator Intro()
    {
        escribiendoBloque = true;
        frase = "¡Ei! ¿Eres tú? Madre mía, ¡cuánto tiempo sin verte! Desde el instituto, ¿verdad? No has cambiado nada, tienes exactamente la misma cara que cuando teníamos 15 años.";
        yield return StartCoroutine (EscribirLento());
        frase = "¡Tenemos que ponernos al día! Llevas demasiado tiempo fuera de la ciudad, han pasado muchas cosas…";
        yield return StartCoroutine (EscribirLento());
        laRuleta();
        escribiendoBloque = false;
    }

    IEnumerator Bloque1()
    {
        escribiendoBloque = true;
        frase = "Nuevo curro en la tienda ¿eh?. Tranquilo, no todos estamos hechos para el éxito ¿sabes a lo que me refiero?";
        yield return StartCoroutine(EscribirLento());
        frase = "Hay gente que tiene que suplir su falta de talento de alguna manera… no todos podemos ganarnos la vida haciendo cosas interesantes.";
        yield return StartCoroutine(EscribirLento());
        if (emocion == "neutral")
        {
            frase = "Al final es como funciona el mundo, nos guste o no.";
            exito += 0.1f;
        }
        else if (emocion == "feliz")
        {
           frase = "Sabía que lo entenderías, no todo el mundo está hecho para triunfar.";
           exito += 0.3f;
        }
        else if (emocion == "enfadado")
        {
            frase = "No pretendía ofender, eh? Que sensible te has vuelto…";
            exito -= 0.3f;
        }
        else if (emocion == "triste")
        {
           frase = "Pero hombre, ¡no te pongas triste! Este curro es temporal, ¿verdad?";
           exito -= 0.1f;
        }
        else if (emocion == "sorprendido")
        {
            frase = "¿Ah? ¿No estás de acuerdo? Pues así es como funciona el mundo";
            exito -= 0.1f;
        }
        yield return StartCoroutine(EscribirLento());
        frase = "Pero bueno, hablando de trabajo… ¡Adivina quién acaba de conseguir un ascenso!";
        yield return StartCoroutine(EscribirLento());
        frase = "Un poco tarde si me preguntas, ya era hora de que alguien se diese cuenta de lo importante que soy para la empresa ¡estarían perdidos sin mí!";
        yield return StartCoroutine(EscribirLento());
        frase = "¿Que donde trabajo? En una firma de modelos claro, ¡el lugar indicado para una chica como yo!";
        yield return StartCoroutine(EscribirLento());
        if (emocion == "neutral")
        {
            frase = "Podrías alegrarte un poco más, tu falta de éxito en la vida no es culpa mía.";
            exito -= 0.2f;
        }
        else if (emocion == "feliz")
        {
           frase = "La vida acaba poniendo a cada uno en su lugar, y el mío está bastante guay.";
           exito += 0.2f;
        }
        else if (emocion == "enfadado")
        {
            frase = "¿Qué? ¿Envidioso de mi increíble vida?";
            exito -= 0.1f;
        }
        else if (emocion == "triste")
        {
           frase = "¡No te apures! No todos estamos hechos de la misma pasta, no hay que avergonzarse de ser mediocre.";
           exito -= 0.2f;
        }
        else if (emocion == "sorprendido")
        {
            frase = "Es normal que te sorprenda, la gente suele quedarse sin palabras ante mi genialidad.";
            exito += 0.1f;
        }
        yield return StartCoroutine(EscribirLento());
        laRuleta();
        escribiendoBloque = false;
    }






















    

    
    //ELIGE UN BLOQUE DE DIALOGO AL AZAR QUE NO HAYA SALIDO ANTES
    //Tenemos una lista de bloques. Elige uno de los disponibles y lo borra de la lista para que no se repita.

    void laRuleta()
    {
        //Por seguridad. No debería poder pasar, pero si la lista está vacía, se rellena para evitar un error.
        if (bloquesDisponibles.Count == 0)
        {
            for (int i = minListaBloques; i < maxListaBloques; i++)
            {
                bloquesDisponibles.Add(i);
            }
        }


        //Elige un bloque al azar de la lista, lo asigna como bloque elegido y lo borra de la lista.
        bloqueIdentificador = Random.Range(minListaBloques, maxListaBloques);
        if (bloquesDisponibles.Contains(bloqueIdentificador))
        {
            bloquesDisponibles.Remove(bloqueIdentificador);
        }
        else
        {
            laRuleta();
        }

    }



}
*/

/*
        if (emocion == "neutral")
        {
            frase = "";
            exito += 0.0f;
        }
        else if (emocion == "feliz")
        {
           frase = "";
           exito += 0.0f;
        }
        else if (emocion == "enfadado")
        {
            frase = "";
            exito += 0.0f;
        }
        else if (emocion == "triste")
        {
           frase = "";
           exito += 0.0f;
        }
        else if (emocion == "sorprendido")
        {
            frase = "";
            exito += 0.0f;
        }





        yield return StartCoroutine(EscribirLento());
        */
