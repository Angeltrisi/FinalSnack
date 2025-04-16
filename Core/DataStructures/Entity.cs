namespace FinalSnack.Core.DataStructures
{
    public abstract class Entity
    {
        public Vector2 position;
        public Vector2 velocity;
        public int width;
        public int height;
        public bool active = false;
        public ushort whoAmI = 0;
        public Vector2 Size
        {
            get
            {
                return new Vector2(width, height);
            }
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }
        public Vector2 Center
        {
            get
            {
                return position + (Size * 0.5f);
            }
            set
            {
                position = value - (Size * 0.5f);
            }
        }
        public Rectangle Hitbox => new(position.ToPoint(), new Point(width, height));

        #region Location Helpers
        public Vector2 TopLeft => position;
        public Vector2 Top => position + new Vector2(width * 0.5f, 0f);
        public Vector2 TopRight => position + new Vector2(width, 0f);
        public Vector2 CenterRight => position + new Vector2(width, height * 0.5f);
        public Vector2 BottomRight => position + Size;
        public Vector2 Bottom => position + new Vector2(width * 0.5f, height);
        public Vector2 BottomLeft => position + new Vector2(0f, height);
        public Vector2 CenterLeft => position + new Vector2(0f, height * 0.5f);
        #endregion

        protected Entity()
        {
            active = true;
            Entity[] entities = Main.EntitiesNeedToUpdate;
            bool added = false;
            for (byte i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                if (entity is null || !entity.active)
                {
                    whoAmI = i;
                    entities[i] = this;
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                active = false;
                return;
            }
            Init();
        }
        /// <summary>
        /// Think SetDefaults in Terraria
        /// </summary>
        public virtual void Init()
        {

        }
        public virtual void Update(GameTime gt)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gt)
        {

        }
    }
}
