﻿using Inventario.Models.Dao;
using Inventario.Models.DTO;
using Inventario.Models.iDAO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventario.Models.DAO
{
    class DAOProducto : iDAOProducto
    {
        private BdContext database;
        private MySqlConnection conexionbd;
        private string declaracion;
        private MySqlDataReader reader;
        private List<DTOProducto> productos;

        public DAOProducto()
        {
            this.database = null;

            this.reader = null;
            this.declaracion = "";

        }


        /* Metodos implementados de iDAOProducto */

        /* Patron de diseño SINGLETON */
        public BdContext ConectarBD()
        {
            if (database == null)
            {
                this.database = new BdContext();
            }

            return database;
        }

        /* Actualiza la List local de los productos con la informacion de la BD*/
        private List<DTOProducto> actualizarProductosLocalmente()
        {
            this.productos = new List<DTOProducto>();
            this.ConectarBD();
            declaracion = "Select * FROM producto";
            this.reader = this.database.Consultar(declaracion);
            this.productos = new List<DTOProducto>();
            while (this.reader.Read())
            {
                productos.Add(new DTOProducto(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
            }
            this.database.CerrarConexion();
            return productos;
        }

        /* Metodo que busca en la lista de productos si se 
           encuentra un producto con el nombre pasado en parametro */
        public DTOProducto BuscarProductoNombre(string nombre)
        {
            foreach (DTOProducto producto in productos)
            {
                if (producto.NombreProducto.Equals(nombre))
                {
                    return producto;
                }
            }
            return null;
        }
        /* Metodo que busca en la lista de productos si se 
          encuentra un producto con el numero de serie pasado en parametro */
        public DTOProducto BuscarProductoNumSerie(int numSerie){
            foreach (DTOProducto producto in productos){
                if (producto.NumSerie==numSerie){
                    return producto;
                }
            }
            return null;
        }

        /* Devuelve la lista de productos local */
        public List<DTOProducto> darProductos()
        {
            this.actualizarProductosLocalmente();
            return productos;
        }

        /* Crea producto nuevo */
        public Boolean CrearProducto(string nombreProducto, string proveedor, string categoria, int precioUnidad, int cantidadExistente)
        {
            try
            {
                this.ConectarBD();
                declaracion = "INSERT INTO Producto(nombreproducto, proveedor, categoria, preciounidad, cantidadexistente) VALUES" +
                    "('" + nombreProducto + "','" + proveedor + "','" + categoria + "'," + precioUnidad + "," + cantidadExistente + ");";
                Console.WriteLine(declaracion);
                this.database.Alterar(declaracion); //Inserta el producto en la BD
                this.database.CerrarConexion();

                if (this.productos == null)
                {
                    this.productos = new List<DTOProducto>();
                }
                this.productos.Add(this.BuscarProductoNombre(nombreProducto)); //Crea el producto local

                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());

            }
            this.database.CerrarConexion();
            return false;
        }

        /* Elimina un producto */
        public Boolean EliminarProducto(int numserie)
        {
            this.ConectarBD();
            declaracion = "DELETE FROM Producto WHERE num_serie = " + numserie;
            if (this.database.Alterar(declaracion))
            {
                //Elimina de la BD
                this.database.CerrarConexion();
                this.actualizarProductosLocalmente(); //Actualiza la lista local
                return true;
            }

            this.database.CerrarConexion();
            return false;
        }

        /* Actualizar producto */
        public Boolean ActualizarProducto(int numeroserie, string nombreproducto, string proveedor, string categoria, int preciounidad, int cantidadexistente)
        {
            this.ConectarBD();
            
            declaracion = "UPDATE Producto SET nombreproducto='" + nombreproducto + "', proveedor='" + proveedor + "', categoria='" + categoria + "', " +
                "preciounidad=" + preciounidad + ", cantidadexistente=" + cantidadexistente + " WHERE num_serie = "+numeroserie+";";
            if (this.database.Alterar(declaracion))
            {
                this.database.CerrarConexion(); 
                this.actualizarProductosLocalmente(); //Actualiza localmente
                return true;

            }
            this.database.CerrarConexion();
            return false;
        }
    }
}
