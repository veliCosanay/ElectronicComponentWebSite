PGDMP      +                |            KomponentService    14.13    16.4 U    Y           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            Z           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            [           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            \           1262    41428    KomponentService    DATABASE     �   CREATE DATABASE "KomponentService" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Turkish_T�rkiye.1254';
 "   DROP DATABASE "KomponentService";
                postgres    false                        2615    2200    public    SCHEMA     2   -- *not* creating schema, since initdb creates it
 2   -- *not* dropping schema, since initdb creates it
                postgres    false            ]           0    0    SCHEMA public    ACL     Q   REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;
                   postgres    false    4            �            1255    41530 K   category_crud(character varying, integer, character varying, text, boolean)    FUNCTION     �  CREATE FUNCTION public.category_crud(p_action character varying, p_categoryid integer DEFAULT NULL::integer, p_categoryname character varying DEFAULT NULL::character varying, p_categoryimageurl text DEFAULT NULL::text, p_isactive boolean DEFAULT NULL::boolean) RETURNS TABLE(out_categoryid integer, out_categoryname character varying, out_categoryimageurl text, out_isactive boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF p_Action IS NULL THEN
        RAISE EXCEPTION 'Action parameter cannot be NULL';
    END IF;

    CASE UPPER(p_Action)
        WHEN 'GETALL' THEN
            RETURN QUERY 
            SELECT 
                c.CategoryId as out_CategoryId, 
                c.CategoryName as out_CategoryName, 
                c.CategoryImageUrl as out_CategoryImageUrl, 
                c.IsActive as out_IsActive
            FROM Category c
            ORDER BY c.CategoryId;

        WHEN 'INSERT' THEN
            IF p_CategoryName IS NULL THEN
                RAISE EXCEPTION 'CategoryName is required for INSERT';
            END IF;

            RETURN QUERY
            WITH inserted AS (
                INSERT INTO Category(
                    CategoryName, 
                    CategoryImageUrl, 
                    IsActive
                ) VALUES (
                    p_CategoryName, 
                    p_CategoryImageUrl, 
                    COALESCE(p_IsActive, FALSE)
                )
                RETURNING 
                    CategoryId as out_CategoryId, 
                    CategoryName as out_CategoryName, 
                    CategoryImageUrl as out_CategoryImageUrl, 
                    IsActive as out_IsActive
            )
            SELECT * FROM inserted;

			WHEN 'GETBYID' THEN
            IF p_CategoryId IS NULL THEN
                RAISE EXCEPTION 'CategoryId is required for GETBYID';
            END IF;

            RETURN QUERY 
            SELECT 
                c.CategoryId as out_CategoryId, 
                c.CategoryName as out_CategoryName, 
                c.CategoryImageUrl as out_CategoryImageUrl, 
                c.IsActive as out_IsActive
            FROM Category c
            WHERE c.CategoryId = p_CategoryId;

        WHEN 'UPDATE' THEN
            IF p_CategoryId IS NULL THEN
                RAISE EXCEPTION 'CategoryId is required for UPDATE';
            END IF;

            RETURN QUERY
            WITH updated AS (
                UPDATE Category
                SET CategoryName = COALESCE(p_CategoryName, CategoryName),
                    CategoryImageUrl = COALESCE(p_CategoryImageUrl, CategoryImageUrl),
                    IsActive = COALESCE(p_IsActive, IsActive)
                WHERE CategoryId = p_CategoryId
                RETURNING 
                    CategoryId as out_CategoryId, 
                    CategoryName as out_CategoryName, 
                    CategoryImageUrl as out_CategoryImageUrl, 
                    IsActive as out_IsActive
            )
            SELECT * FROM updated;

        WHEN 'DELETE' THEN
            IF p_CategoryId IS NULL THEN
                RAISE EXCEPTION 'CategoryId is required for DELETE';
            END IF;

            DELETE FROM Category 
            WHERE CategoryId = p_CategoryId;
            RETURN;

        WHEN 'ACTIVECATEGORY' THEN
            RETURN QUERY 
            SELECT 
                c.CategoryId as out_CategoryId, 
                c.CategoryName as out_CategoryName, 
                c.CategoryImageUrl as out_CategoryImageUrl, 
                c.IsActive as out_IsActive
            FROM Category c
            WHERE c.IsActive = TRUE 
            ORDER BY c.CategoryName;

        ELSE
            RAISE EXCEPTION 'Invalid action: %', p_Action;
    END CASE;
END;
$$;
 �   DROP FUNCTION public.category_crud(p_action character varying, p_categoryid integer, p_categoryname character varying, p_categoryimageurl text, p_isactive boolean);
       public          postgres    false    4            �            1255    41605    delete_users_by_roleid(integer) 	   PROCEDURE     �   CREATE PROCEDURE public.delete_users_by_roleid(IN role_id integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM Users
    WHERE roleid = role_id;
END;
$$;
 B   DROP PROCEDURE public.delete_users_by_roleid(IN role_id integer);
       public          postgres    false    4            �            1255    41548 �   product_crud(character varying, integer, character varying, character varying, text, numeric, character varying, integer, integer, boolean, text)    FUNCTION     k  CREATE FUNCTION public.product_crud(c_action character varying, c_productid integer DEFAULT NULL::integer, c_productname character varying DEFAULT NULL::character varying, c_shortdescription character varying DEFAULT NULL::character varying, c_longdescription text DEFAULT NULL::text, c_price numeric DEFAULT NULL::numeric, c_companyname character varying DEFAULT NULL::character varying, c_categoryid integer DEFAULT NULL::integer, c_stock integer DEFAULT NULL::integer, c_isactive boolean DEFAULT NULL::boolean, c_productimageurl text DEFAULT NULL::text) RETURNS TABLE(out_productid integer, out_productname character varying, out_shortdescription character varying, out_longdescription text, out_price numeric, out_companyname character varying, out_categoryid integer, out_categoryname character varying, out_stock integer, out_isactive boolean, out_productimageurl text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF c_Action IS NULL THEN
        RAISE EXCEPTION 'Action parameter cannot be NULL';
    END IF;

    CASE UPPER(c_Action)
        WHEN 'GETALL' THEN
            RETURN QUERY 
            SELECT 
                p.ProductId as out_productid,
                p.ProductName as out_productname,
                p.ShortDescription as out_shortdescription,
                p.LongDescription as out_longdescription,
                p.Price as out_price,
                p.CompanyName as out_companyname,
                p.CategoryId as out_categoryid,
                c.CategoryName as out_categoryname,
                p.Stock as out_stock,
                p.IsActive as out_isactive,
                p.ProductImageUrl as out_productimageurl
            FROM Product p
            LEFT JOIN Category c ON p.CategoryId = c.CategoryId
            ORDER BY p.ProductId;

        WHEN 'GETBYID' THEN
            IF c_ProductId IS NULL THEN
                RAISE EXCEPTION 'ProductId is required for GETBYID';
            END IF;

            RETURN QUERY 
            SELECT 
                p.ProductId as out_productid,
                p.ProductName as out_productname,
                p.ShortDescription as out_shortdescription,
                p.LongDescription as out_longdescription,
                p.Price as out_price,
                p.CompanyName as out_companyname,
                p.CategoryId as out_categoryid,
                c.CategoryName as out_categoryname,
                p.Stock as out_stock,
                p.IsActive as out_isactive,
                p.ProductImageUrl as out_productimageurl
            FROM Product p
            LEFT JOIN Category c ON p.CategoryId = c.CategoryId
            WHERE p.ProductId = c_ProductId;

        WHEN 'INSERT' THEN
            IF c_ProductName IS NULL THEN
                RAISE EXCEPTION 'ProductName is required for INSERT';
            END IF;

            RETURN QUERY
            WITH inserted AS (
                INSERT INTO Product(
                    ProductName,
                    ShortDescription,
                    LongDescription,
                    Price,
                    CompanyName,
                    CategoryId,
                    Stock,
                    IsActive,
                    ProductImageUrl
                ) VALUES (
                    c_ProductName,
                    c_ShortDescription,
                    c_LongDescription,
                    c_Price,
                    c_CompanyName,
                    c_CategoryId,
                    c_Stock,
                    COALESCE(c_IsActive, FALSE),
                    c_ProductImageUrl
                )
                RETURNING 
                    ProductId,
                    ProductName,
                    ShortDescription,
                    LongDescription,
                    Price,
                    CompanyName,
                    CategoryId,
                    Stock,
                    IsActive,
                    ProductImageUrl
            )
            SELECT 
                i.ProductId as out_productid,
                i.ProductName as out_productname,
                i.ShortDescription as out_shortdescription,
                i.LongDescription as out_longdescription,
                i.Price as out_price,
                i.CompanyName as out_companyname,
                i.CategoryId as out_categoryid,
                c.CategoryName as out_categoryname,
                i.Stock as out_stock,
                i.IsActive as out_isactive,
                i.ProductImageUrl as out_productimageurl
            FROM inserted i
            LEFT JOIN Category c ON i.CategoryId = c.CategoryId;

        WHEN 'UPDATE' THEN
            IF c_ProductId IS NULL THEN
                RAISE EXCEPTION 'ProductId is required for UPDATE';
            END IF;

            RETURN QUERY
            WITH updated AS (
                UPDATE Product
                SET 
                    ProductName = COALESCE(c_ProductName, ProductName),
                    ShortDescription = COALESCE(c_ShortDescription, ShortDescription),
                    LongDescription = COALESCE(c_LongDescription, LongDescription),
                    Price = COALESCE(c_Price, Price),
                    CompanyName = COALESCE(c_CompanyName, CompanyName),
                    CategoryId = COALESCE(c_CategoryId, CategoryId),
                    Stock = COALESCE(c_Stock, Stock),
                    IsActive = COALESCE(c_IsActive, IsActive),
                    ProductImageUrl = COALESCE(c_ProductImageUrl, ProductImageUrl)
                WHERE ProductId = c_ProductId
                RETURNING 
                    ProductId,
                    ProductName,
                    ShortDescription,
                    LongDescription,
                    Price,
                    CompanyName,
                    CategoryId,
                    Stock,
                    IsActive,
                    ProductImageUrl
            )
            SELECT 
                u.ProductId as out_productid,
                u.ProductName as out_productname,
                u.ShortDescription as out_shortdescription,
                u.LongDescription as out_longdescription,
                u.Price as out_price,
                u.CompanyName as out_companyname,
                u.CategoryId as out_categoryid,
                c.CategoryName as out_categoryname,
                u.Stock as out_stock,
                u.IsActive as out_isactive,
                u.ProductImageUrl as out_productimageurl
            FROM updated u
            LEFT JOIN Category c ON u.CategoryId = c.CategoryId;

        WHEN 'DELETE' THEN
            IF c_ProductId IS NULL THEN
                RAISE EXCEPTION 'ProductId is required for DELETE';
            END IF;

            DELETE FROM Product 
            WHERE ProductId = c_ProductId;
            
            RETURN QUERY 
            SELECT 
                NULL::INT as out_productid,
                NULL::VARCHAR(100) as out_productname,
                NULL::VARCHAR(100) as out_shortdescription,
                NULL::TEXT as out_longdescription,
                NULL::DECIMAL(18,2) as out_price,
                NULL::VARCHAR(100) as out_companyname,
                NULL::INT as out_categoryid,
                NULL::VARCHAR(100) as out_categoryname,
                NULL::INT as out_stock,
                NULL::BOOLEAN as out_isactive,
                NULL::TEXT as out_productimageurl
            WHERE FALSE;

        ELSE
            RAISE EXCEPTION 'Invalid action: %', c_Action;
    END CASE;
END;
$$;
 8  DROP FUNCTION public.product_crud(c_action character varying, c_productid integer, c_productname character varying, c_shortdescription character varying, c_longdescription text, c_price numeric, c_companyname character varying, c_categoryid integer, c_stock integer, c_isactive boolean, c_productimageurl text);
       public          postgres    false    4            �            1255    41601    switch_max_price()    FUNCTION     �   CREATE FUNCTION public.switch_max_price() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.Price > 999999.99 THEN
        NEW.Price := 999999.99;
    END IF;
    RETURN NEW;
END;
$$;
 )   DROP FUNCTION public.switch_max_price();
       public          postgres    false    4            �            1255    41603    switch_min_price()    FUNCTION     �   CREATE FUNCTION public.switch_min_price() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.Price < 1.5 THEN
        NEW.Price := 1.5;
    END IF;
    RETURN NEW;
END;
$$;
 )   DROP FUNCTION public.switch_min_price();
       public          postgres    false    4            �            1255    41549 �   user_crud(character varying, integer, character varying, character varying, character varying, text, character varying, text, integer)    FUNCTION     R  CREATE FUNCTION public.user_crud(c_action character varying, c_userid integer DEFAULT NULL::integer, c_name character varying DEFAULT NULL::character varying, c_mobile character varying DEFAULT NULL::character varying, c_email character varying DEFAULT NULL::character varying, c_address text DEFAULT NULL::text, c_password character varying DEFAULT NULL::character varying, c_imageurl text DEFAULT NULL::text, c_roleid integer DEFAULT NULL::integer) RETURNS TABLE(out_userid integer, out_name character varying, out_mobile character varying, out_email character varying, out_address text, out_password character varying, out_imageurl text, out_roleid integer, out_rolename character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF c_Action IS NULL THEN
        RAISE EXCEPTION 'Action parameter cannot be NULL';
    END IF;

    CASE UPPER(c_Action)
        WHEN 'GETALL' THEN
            RETURN QUERY 
            SELECT 
                u.UserId as out_userid,
                u.Name as out_name,
                u.Mobile as out_mobile,
                u.Email as out_email,
                u.Address as out_address,
                u.Password as out_password,
                u.ImageUrl as out_imageurl,
                u.RoleId as out_roleid,
                r.RoleName as out_rolename
            FROM Users u
            LEFT JOIN Roles r ON u.RoleId = r.RoleId
            ORDER BY u.UserId;

        WHEN 'GETBYID' THEN
            IF c_UserId IS NULL THEN
                RAISE EXCEPTION 'UserId is required for GETBYID';
            END IF;

            RETURN QUERY 
            SELECT 
                u.UserId as out_userid,
                u.Name as out_name,
                u.Mobile as out_mobile,
                u.Email as out_email,
                u.Address as out_address,
                u.Password as out_password,
                u.ImageUrl as out_imageurl,
                u.RoleId as out_roleid,
                r.RoleName as out_rolename
            FROM Users u
            LEFT JOIN Roles r ON u.RoleId = r.RoleId
            WHERE u.UserId = c_UserId;

        WHEN 'INSERT' THEN
            IF c_Name IS NULL OR c_Password IS NULL THEN
                RAISE EXCEPTION 'Name and Password are required for INSERT';
            END IF;

            RETURN QUERY
            WITH inserted AS (
                INSERT INTO Users(
                    Name,
                    Mobile,
                    Email,
                    Address,
                    Password,
                    ImageUrl,
                    RoleId
                ) VALUES (
                    c_Name,
                    c_Mobile,
                    c_Email,
                    c_Address,
                    c_Password,  -- Gerçek uygulamada şifre hash'lenmelidir
                    c_ImageUrl,
                    c_RoleId
                )
                RETURNING *
            )
            SELECT 
                i.UserId as out_userid,
                i.Name as out_name,
                i.Mobile as out_mobile,
                i.Email as out_email,
                i.Address as out_address,
                i.Password as out_password,
                i.ImageUrl as out_imageurl,
                i.RoleId as out_roleid,
                r.RoleName as out_rolename
            FROM inserted i
            LEFT JOIN Roles r ON i.RoleId = r.RoleId;

        WHEN 'UPDATE' THEN
            IF c_UserId IS NULL THEN
                RAISE EXCEPTION 'UserId is required for UPDATE';
            END IF;

            RETURN QUERY
            WITH updated AS (
                UPDATE Users
                SET 
                    Name = COALESCE(c_Name, Name),
                    Mobile = COALESCE(c_Mobile, Mobile),
                    Email = COALESCE(c_Email, Email),
                    Address = COALESCE(c_Address, Address),
                    Password = COALESCE(c_Password, Password),
                    ImageUrl = COALESCE(c_ImageUrl, ImageUrl),
                    RoleId = COALESCE(c_RoleId, RoleId)
                WHERE UserId = c_UserId
                RETURNING *
            )
            SELECT 
                u.UserId as out_userid,
                u.Name as out_name,
                u.Mobile as out_mobile,
                u.Email as out_email,
                u.Address as out_address,
                u.Password as out_password,
                u.ImageUrl as out_imageurl,
                u.RoleId as out_roleid,
                r.RoleName as out_rolename
            FROM updated u
            LEFT JOIN Roles r ON u.RoleId = r.RoleId;

        WHEN 'DELETE' THEN
            IF c_UserId IS NULL THEN
                RAISE EXCEPTION 'UserId is required for DELETE';
            END IF;

            DELETE FROM Users 
            WHERE UserId = c_UserId;
            
            RETURN QUERY 
            SELECT 
                NULL::INT as out_userid,
                NULL::VARCHAR(100) as out_name,
                NULL::VARCHAR(20) as out_mobile,
                NULL::VARCHAR(100) as out_email,
                NULL::TEXT as out_address,
                NULL::VARCHAR(100) as out_password,
                NULL::TEXT as out_imageurl,
                NULL::INT as out_roleid,
                NULL::VARCHAR(50) as out_rolename
            WHERE FALSE;

        WHEN 'LOGIN' THEN
            IF c_Email IS NULL OR c_Password IS NULL THEN
                RAISE EXCEPTION 'Email and Password are required for LOGIN';
            END IF;

            RETURN QUERY 
            SELECT 
                u.UserId as out_userid,
                u.Name as out_name,
                u.Mobile as out_mobile,
                u.Email as out_email,
                u.Address as out_address,
                u.Password as out_password,
                u.ImageUrl as out_imageurl,
                u.RoleId as out_roleid,
                r.RoleName as out_rolename
            FROM Users u
            LEFT JOIN Roles r ON u.RoleId = r.RoleId
            WHERE u.Email = c_Email 
            AND u.Password = c_Password;  -- Gerçek uygulamada şifre karşılaştırması hash ile yapılmalıdır

        ELSE
            RAISE EXCEPTION 'Invalid action: %', c_Action;
    END CASE;
END;
$$;
 �   DROP FUNCTION public.user_crud(c_action character varying, c_userid integer, c_name character varying, c_mobile character varying, c_email character varying, c_address text, c_password character varying, c_imageurl text, c_roleid integer);
       public          postgres    false    4            �            1259    41551    cart    TABLE     �   CREATE TABLE public.cart (
    cartid integer NOT NULL,
    productid integer NOT NULL,
    quantity integer NOT NULL,
    userid integer NOT NULL,
    CONSTRAINT cart_quantity_check CHECK ((quantity > 0))
);
    DROP TABLE public.cart;
       public         heap    postgres    false    4            �            1259    41550    cart_cartid_seq    SEQUENCE     �   CREATE SEQUENCE public.cart_cartid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 &   DROP SEQUENCE public.cart_cartid_seq;
       public          postgres    false    4    221            ^           0    0    cart_cartid_seq    SEQUENCE OWNED BY     C   ALTER SEQUENCE public.cart_cartid_seq OWNED BY public.cart.cartid;
          public          postgres    false    220            �            1259    41430    category    TABLE     �   CREATE TABLE public.category (
    categoryid integer NOT NULL,
    categoryname character varying(100) NOT NULL,
    categoryimageurl text,
    isactive boolean NOT NULL
);
    DROP TABLE public.category;
       public         heap    postgres    false    4            �            1259    41429    category_categoryid_seq    SEQUENCE     �   CREATE SEQUENCE public.category_categoryid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 .   DROP SEQUENCE public.category_categoryid_seq;
       public          postgres    false    210    4            _           0    0    category_categoryid_seq    SEQUENCE OWNED BY     S   ALTER SEQUENCE public.category_categoryid_seq OWNED BY public.category.categoryid;
          public          postgres    false    209            �            1259    41607    contact    TABLE     
  CREATE TABLE public.contact (
    contactid integer NOT NULL,
    email character varying(50),
    subject character varying(50),
    message text,
    userid integer NOT NULL,
    address character varying(50) NOT NULL,
    mobile character varying(50) NOT NULL
);
    DROP TABLE public.contact;
       public         heap    postgres    false    4            �            1259    41606    contact_contactid_seq    SEQUENCE     �   CREATE SEQUENCE public.contact_contactid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ,   DROP SEQUENCE public.contact_contactid_seq;
       public          postgres    false    4    225            `           0    0    contact_contactid_seq    SEQUENCE OWNED BY     O   ALTER SEQUENCE public.contact_contactid_seq OWNED BY public.contact.contactid;
          public          postgres    false    224            �            1259    41512    orders    TABLE     �  CREATE TABLE public.orders (
    orderdetailsid integer NOT NULL,
    orderno text NOT NULL,
    productid integer NOT NULL,
    quantity integer NOT NULL,
    userid integer NOT NULL,
    status character varying(50),
    paymentid integer NOT NULL,
    orderdate timestamp without time zone NOT NULL,
    iscancel boolean NOT NULL,
    CONSTRAINT check_quantity_positive CHECK ((quantity > 0)),
    CONSTRAINT check_status_values CHECK (((status)::text = ANY ((ARRAY['Pending'::character varying, 'Processing'::character varying, 'Shipped'::character varying, 'Delivered'::character varying, 'Cancelled'::character varying])::text[])))
);
    DROP TABLE public.orders;
       public         heap    postgres    false    4            �            1259    41511    orders_orderdetailsid_seq    SEQUENCE     �   CREATE SEQUENCE public.orders_orderdetailsid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.orders_orderdetailsid_seq;
       public          postgres    false    4    219            a           0    0    orders_orderdetailsid_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.orders_orderdetailsid_seq OWNED BY public.orders.orderdetailsid;
          public          postgres    false    218            �            1259    41573    payment    TABLE     +  CREATE TABLE public.payment (
    paymentid integer NOT NULL,
    name character varying(50) NOT NULL,
    cardno character varying(50) NOT NULL,
    expirydate character varying(10) NOT NULL,
    cvvno character varying(5) NOT NULL,
    address text NOT NULL,
    paymentmode character varying(50) NOT NULL,
    paymentstatus character varying(50) DEFAULT 'Pending'::character varying NOT NULL,
    userid integer NOT NULL,
    amount numeric(18,2) NOT NULL,
    createddate timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    orderdetails text NOT NULL,
    CONSTRAINT payment_amount_check CHECK ((amount > (0)::numeric)),
    CONSTRAINT payment_paymentmode_check CHECK (((paymentmode)::text = ANY ((ARRAY['Credit Card'::character varying, 'Debit Card'::character varying, 'Cash On Delivery'::character varying])::text[]))),
    CONSTRAINT payment_paymentstatus_check CHECK (((paymentstatus)::text = ANY ((ARRAY['Pending'::character varying, 'Completed'::character varying, 'Failed'::character varying, 'Refunded'::character varying])::text[])))
);
    DROP TABLE public.payment;
       public         heap    postgres    false    4            �            1259    41572    payment_paymentid_seq    SEQUENCE     �   CREATE SEQUENCE public.payment_paymentid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ,   DROP SEQUENCE public.payment_paymentid_seq;
       public          postgres    false    4    223            b           0    0    payment_paymentid_seq    SEQUENCE OWNED BY     O   ALTER SEQUENCE public.payment_paymentid_seq OWNED BY public.payment.paymentid;
          public          postgres    false    222            �            1259    41439    product    TABLE     o  CREATE TABLE public.product (
    productid integer NOT NULL,
    productname character varying(100) NOT NULL,
    shortdescription character varying(200),
    longdescription text,
    price numeric(18,2) NOT NULL,
    companyname character varying(100),
    categoryid integer NOT NULL,
    stock integer,
    isactive boolean NOT NULL,
    productimageurl text
);
    DROP TABLE public.product;
       public         heap    postgres    false    4            �            1259    41438    product_productid_seq    SEQUENCE     �   CREATE SEQUENCE public.product_productid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ,   DROP SEQUENCE public.product_productid_seq;
       public          postgres    false    212    4            c           0    0    product_productid_seq    SEQUENCE OWNED BY     O   ALTER SEQUENCE public.product_productid_seq OWNED BY public.product.productid;
          public          postgres    false    211            �            1259    41474    productreview    TABLE     �   CREATE TABLE public.productreview (
    reviewid integer NOT NULL,
    rating integer NOT NULL,
    comment text,
    productid integer NOT NULL,
    userid integer NOT NULL
);
 !   DROP TABLE public.productreview;
       public         heap    postgres    false    4            �            1259    41473    productreview_reviewid_seq    SEQUENCE     �   CREATE SEQUENCE public.productreview_reviewid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 1   DROP SEQUENCE public.productreview_reviewid_seq;
       public          postgres    false    217    4            d           0    0    productreview_reviewid_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.productreview_reviewid_seq OWNED BY public.productreview.reviewid;
          public          postgres    false    216            �            1259    41452    roles    TABLE     h   CREATE TABLE public.roles (
    roleid integer NOT NULL,
    rolename character varying(50) NOT NULL
);
    DROP TABLE public.roles;
       public         heap    postgres    false    4            �            1259    41458    users    TABLE     B  CREATE TABLE public.users (
    userid integer NOT NULL,
    name character varying(50) NOT NULL,
    password character varying(50) NOT NULL,
    imageurl text,
    roleid integer NOT NULL,
    isactive boolean DEFAULT true NOT NULL,
    mobile character varying(12),
    email character varying(50),
    address text
);
    DROP TABLE public.users;
       public         heap    postgres    false    4            �            1259    41457    users_userid_seq    SEQUENCE     �   CREATE SEQUENCE public.users_userid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.users_userid_seq;
       public          postgres    false    215    4            e           0    0    users_userid_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;
          public          postgres    false    214            �           2604    41554    cart cartid    DEFAULT     j   ALTER TABLE ONLY public.cart ALTER COLUMN cartid SET DEFAULT nextval('public.cart_cartid_seq'::regclass);
 :   ALTER TABLE public.cart ALTER COLUMN cartid DROP DEFAULT;
       public          postgres    false    221    220    221            �           2604    41433    category categoryid    DEFAULT     z   ALTER TABLE ONLY public.category ALTER COLUMN categoryid SET DEFAULT nextval('public.category_categoryid_seq'::regclass);
 B   ALTER TABLE public.category ALTER COLUMN categoryid DROP DEFAULT;
       public          postgres    false    210    209    210            �           2604    41610    contact contactid    DEFAULT     v   ALTER TABLE ONLY public.contact ALTER COLUMN contactid SET DEFAULT nextval('public.contact_contactid_seq'::regclass);
 @   ALTER TABLE public.contact ALTER COLUMN contactid DROP DEFAULT;
       public          postgres    false    225    224    225            �           2604    41515    orders orderdetailsid    DEFAULT     ~   ALTER TABLE ONLY public.orders ALTER COLUMN orderdetailsid SET DEFAULT nextval('public.orders_orderdetailsid_seq'::regclass);
 D   ALTER TABLE public.orders ALTER COLUMN orderdetailsid DROP DEFAULT;
       public          postgres    false    219    218    219            �           2604    41576    payment paymentid    DEFAULT     v   ALTER TABLE ONLY public.payment ALTER COLUMN paymentid SET DEFAULT nextval('public.payment_paymentid_seq'::regclass);
 @   ALTER TABLE public.payment ALTER COLUMN paymentid DROP DEFAULT;
       public          postgres    false    222    223    223            �           2604    41442    product productid    DEFAULT     v   ALTER TABLE ONLY public.product ALTER COLUMN productid SET DEFAULT nextval('public.product_productid_seq'::regclass);
 @   ALTER TABLE public.product ALTER COLUMN productid DROP DEFAULT;
       public          postgres    false    212    211    212            �           2604    41477    productreview reviewid    DEFAULT     �   ALTER TABLE ONLY public.productreview ALTER COLUMN reviewid SET DEFAULT nextval('public.productreview_reviewid_seq'::regclass);
 E   ALTER TABLE public.productreview ALTER COLUMN reviewid DROP DEFAULT;
       public          postgres    false    216    217    217            �           2604    41461    users userid    DEFAULT     l   ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);
 ;   ALTER TABLE public.users ALTER COLUMN userid DROP DEFAULT;
       public          postgres    false    214    215    215            R          0    41551    cart 
   TABLE DATA           C   COPY public.cart (cartid, productid, quantity, userid) FROM stdin;
    public          postgres    false    221   ��       G          0    41430    category 
   TABLE DATA           X   COPY public.category (categoryid, categoryname, categoryimageurl, isactive) FROM stdin;
    public          postgres    false    210   ��       V          0    41607    contact 
   TABLE DATA           ^   COPY public.contact (contactid, email, subject, message, userid, address, mobile) FROM stdin;
    public          postgres    false    225   �       P          0    41512    orders 
   TABLE DATA           ~   COPY public.orders (orderdetailsid, orderno, productid, quantity, userid, status, paymentid, orderdate, iscancel) FROM stdin;
    public          postgres    false    219   �       T          0    41573    payment 
   TABLE DATA           �   COPY public.payment (paymentid, name, cardno, expirydate, cvvno, address, paymentmode, paymentstatus, userid, amount, createddate, orderdetails) FROM stdin;
    public          postgres    false    223   �       I          0    41439    product 
   TABLE DATA           �   COPY public.product (productid, productname, shortdescription, longdescription, price, companyname, categoryid, stock, isactive, productimageurl) FROM stdin;
    public          postgres    false    212   <�       N          0    41474    productreview 
   TABLE DATA           U   COPY public.productreview (reviewid, rating, comment, productid, userid) FROM stdin;
    public          postgres    false    217   �       J          0    41452    roles 
   TABLE DATA           1   COPY public.roles (roleid, rolename) FROM stdin;
    public          postgres    false    213   ��       L          0    41458    users 
   TABLE DATA           k   COPY public.users (userid, name, password, imageurl, roleid, isactive, mobile, email, address) FROM stdin;
    public          postgres    false    215   ̳       f           0    0    cart_cartid_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('public.cart_cartid_seq', 5, true);
          public          postgres    false    220            g           0    0    category_categoryid_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.category_categoryid_seq', 4, true);
          public          postgres    false    209            h           0    0    contact_contactid_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.contact_contactid_seq', 1, true);
          public          postgres    false    224            i           0    0    orders_orderdetailsid_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.orders_orderdetailsid_seq', 1, false);
          public          postgres    false    218            j           0    0    payment_paymentid_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.payment_paymentid_seq', 5, true);
          public          postgres    false    222            k           0    0    product_productid_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.product_productid_seq', 9, true);
          public          postgres    false    211            l           0    0    productreview_reviewid_seq    SEQUENCE SET     I   SELECT pg_catalog.setval('public.productreview_reviewid_seq', 1, false);
          public          postgres    false    216            m           0    0    users_userid_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.users_userid_seq', 6, true);
          public          postgres    false    214            �           2606    41558    cart cart_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.cart
    ADD CONSTRAINT cart_pkey PRIMARY KEY (cartid);
 8   ALTER TABLE ONLY public.cart DROP CONSTRAINT cart_pkey;
       public            postgres    false    221            �           2606    41437    category category_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_pkey PRIMARY KEY (categoryid);
 @   ALTER TABLE ONLY public.category DROP CONSTRAINT category_pkey;
       public            postgres    false    210            �           2606    41614    contact contact_pkey 
   CONSTRAINT     Y   ALTER TABLE ONLY public.contact
    ADD CONSTRAINT contact_pkey PRIMARY KEY (contactid);
 >   ALTER TABLE ONLY public.contact DROP CONSTRAINT contact_pkey;
       public            postgres    false    225            �           2606    41519    orders orders_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (orderdetailsid);
 <   ALTER TABLE ONLY public.orders DROP CONSTRAINT orders_pkey;
       public            postgres    false    219            �           2606    41585    payment payment_pkey 
   CONSTRAINT     Y   ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_pkey PRIMARY KEY (paymentid);
 >   ALTER TABLE ONLY public.payment DROP CONSTRAINT payment_pkey;
       public            postgres    false    223            �           2606    41446    product product_pkey 
   CONSTRAINT     Y   ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_pkey PRIMARY KEY (productid);
 >   ALTER TABLE ONLY public.product DROP CONSTRAINT product_pkey;
       public            postgres    false    212            �           2606    41481     productreview productreview_pkey 
   CONSTRAINT     d   ALTER TABLE ONLY public.productreview
    ADD CONSTRAINT productreview_pkey PRIMARY KEY (reviewid);
 J   ALTER TABLE ONLY public.productreview DROP CONSTRAINT productreview_pkey;
       public            postgres    false    217            �           2606    41456    roles roles_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (roleid);
 :   ALTER TABLE ONLY public.roles DROP CONSTRAINT roles_pkey;
       public            postgres    false    213            �           2606    41600    orders unique_payment_id 
   CONSTRAINT     X   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT unique_payment_id UNIQUE (paymentid);
 B   ALTER TABLE ONLY public.orders DROP CONSTRAINT unique_payment_id;
       public            postgres    false    219            �           2606    41465    users users_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    215            �           2620    41602    product set_max_price    TRIGGER     �   CREATE TRIGGER set_max_price BEFORE INSERT OR UPDATE ON public.product FOR EACH ROW EXECUTE FUNCTION public.switch_max_price();
 .   DROP TRIGGER set_max_price ON public.product;
       public          postgres    false    212    226            �           2620    41604    product set_min_price    TRIGGER     �   CREATE TRIGGER set_min_price BEFORE INSERT OR UPDATE ON public.product FOR EACH ROW EXECUTE FUNCTION public.switch_min_price();
 .   DROP TRIGGER set_min_price ON public.product;
       public          postgres    false    227    212            �           2606    41559    cart cart_productid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.cart
    ADD CONSTRAINT cart_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(productid) ON DELETE CASCADE;
 B   ALTER TABLE ONLY public.cart DROP CONSTRAINT cart_productid_fkey;
       public          postgres    false    3229    221    212            �           2606    41564    cart cart_userid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.cart
    ADD CONSTRAINT cart_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(userid) ON DELETE CASCADE;
 ?   ALTER TABLE ONLY public.cart DROP CONSTRAINT cart_userid_fkey;
       public          postgres    false    221    215    3233            �           2606    41615    contact fk_contact_user    FK CONSTRAINT     �   ALTER TABLE ONLY public.contact
    ADD CONSTRAINT fk_contact_user FOREIGN KEY (userid) REFERENCES public.users(userid) ON DELETE CASCADE;
 A   ALTER TABLE ONLY public.contact DROP CONSTRAINT fk_contact_user;
       public          postgres    false    225    215    3233            �           2606    41591    orders fk_orders_payment    FK CONSTRAINT     �   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_payment FOREIGN KEY (paymentid) REFERENCES public.payment(paymentid) ON DELETE CASCADE;
 B   ALTER TABLE ONLY public.orders DROP CONSTRAINT fk_orders_payment;
       public          postgres    false    3243    223    219            �           2606    41520    orders orders_productid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(productid) ON DELETE CASCADE;
 F   ALTER TABLE ONLY public.orders DROP CONSTRAINT orders_productid_fkey;
       public          postgres    false    212    3229    219            �           2606    41525    orders orders_userid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(userid) ON DELETE CASCADE;
 C   ALTER TABLE ONLY public.orders DROP CONSTRAINT orders_userid_fkey;
       public          postgres    false    215    219    3233            �           2606    41586    payment payment_userid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(userid) ON DELETE CASCADE;
 E   ALTER TABLE ONLY public.payment DROP CONSTRAINT payment_userid_fkey;
       public          postgres    false    3233    223    215            �           2606    41447    product product_categoryid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_categoryid_fkey FOREIGN KEY (categoryid) REFERENCES public.category(categoryid) ON DELETE CASCADE;
 I   ALTER TABLE ONLY public.product DROP CONSTRAINT product_categoryid_fkey;
       public          postgres    false    3227    210    212            �           2606    41482 *   productreview productreview_productid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productreview
    ADD CONSTRAINT productreview_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(productid) ON DELETE CASCADE;
 T   ALTER TABLE ONLY public.productreview DROP CONSTRAINT productreview_productid_fkey;
       public          postgres    false    212    3229    217            �           2606    41487 '   productreview productreview_userid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productreview
    ADD CONSTRAINT productreview_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(userid) ON DELETE CASCADE;
 Q   ALTER TABLE ONLY public.productreview DROP CONSTRAINT productreview_userid_fkey;
       public          postgres    false    217    3233    215            �           2606    41468    users users_roleid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_roleid_fkey FOREIGN KEY (roleid) REFERENCES public.roles(roleid) ON DELETE CASCADE;
 A   ALTER TABLE ONLY public.users DROP CONSTRAINT users_roleid_fkey;
       public          postgres    false    215    3231    213            R      x������ � �      G   &   x�3�J-�,.�/*���,�2���L.ʇpb���� �	~      V      x������ � �      P      x������ � �      T      x������ � �      I   3   x���LK,N"��bNNC#=NN Ø��3Ə˂(�,g������� �7      N      x������ � �      J       x�3�tL����2�t.-.��M-����� Um      L   5   x�3�,K���442���4�,�4���Cznbf�^r~.gayjQj	W� ���     