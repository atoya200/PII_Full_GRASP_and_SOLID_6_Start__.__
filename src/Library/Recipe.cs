//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Full_GRASP_And_SOLID
{
    public class Recipe : IRecipeContent // Modificado por DIP
    {
        public bool Cooked { get; private set; }
        // Cambiado por OCP
        private IList<BaseStep> steps = new List<BaseStep>();

        public Product FinalProduct { get; set; }

        /// <summary>
        /// Para poder calcular cuando dejo de cocinar en base al tiempo de los pasos
        /// </summary>
        /// <returns></returns>
        private CountdownTimer timer = new CountdownTimer();

        /// <summary>
        /// Creamos un objeto adaptador, para no violar ISP
        /// </summary>
        private TimerAdapter timerClient; 

        /// <summary>
        /// Para inicializar propiedades es que implemento el constructor
        /// </summary>
        public Recipe()
        {
            Cooked = false;
        }
        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }

        /// <summary>
        /// Retorna la suma del tiempo de todos los pasos
        /// </summary>
        /// <returns></returns>
        public int GetCookTime()
        {
            int totalTime = 0;
            foreach (BaseStep step in steps)
            {
                totalTime += step.Time;
            }
            return totalTime;
        }

        public void Cook()
        {
            Console.WriteLine("A cocinar se ha dicho");
            // Crea el objeto adaptador
            this.timerClient = new TimerAdapter(this);
            // Lo registra en el CountdownTimer
            timer.Register(this.GetCookTime(),this.timerClient);
        }

        /// <summary>
        /// Para evitar violar el princio de segregación de depedencias con esta clase, lo mejor es implementar un objeto adaptador
        /// que use esta clase para así no afectar a las demás que dependan de ella. Aplicando así el patrón Adapter, como en el ejemplo 
        /// de la lectura
        /// </summary>
        private class TimerAdapter : TimerClient
        {
            private Recipe recipe;

            public TimerAdapter(Recipe recipe)
            {
                this.recipe = recipe;
            }

            public object TimeOutId { get; }

            /// <summary>
            /// Método implementado al depender de la interfaz TimerClient
            /// </summary>
            public void TimeOut()
            {
                recipe.Cooked = true;
            }
        }
    }
}