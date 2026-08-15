import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  ArrowLeft,
  ArrowRight,
  ChevronRight,
  Cpu,
  Gamepad2,
  HardDrive,
  Headphones,
  Heart,
  Keyboard,
  Laptop,
  MemoryStick,
  Monitor,
  Mouse,
  Network,
  RotateCcw,
  Search,
  ShieldCheck,
  ShoppingBag,
  Sparkles,
  Star,
  Trash2,
  Truck,
} from "lucide-react";
import { useEffect, useState, type FormEvent } from "react";
import {
  Link,
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";
import { ProductCard, ProductVisual } from "../components/ProductCard";
import {
  Button,
  Card,
  EmptyState,
  ErrorState,
  Input,
  PageHeader,
  SkeletonGrid,
  Spinner,
} from "../components/ui";
import { useAuth } from "../core/auth/AuthProvider";
import { formatCurrency, formatDate } from "../core/format";
import { catalogApi, catalogKeys } from "../features/catalog/api";
import { commerceApi, commerceKeys } from "../features/commerce/api";
import { resolveApiAsset } from "../core/api/apiClient";

const heroSlides = [
  {
    eyebrow: "Custom PC builds",
    title: "Build beyond the box.",
    description:
      "Choose a complete system or find the components that make your ideal computer unmistakably yours.",
    image: "/images/hero-custom-pc.png",
    href: "/products?page=1&pageSize=12",
    cta: "Explore all hardware",
  },
  {
    eyebrow: "ASUS performance",
    title: "Power that keeps up with your next move.",
    description:
      "Explore ASUS laptops built for competitive play, demanding creative work, and everything between.",
    image: "/images/hero-asus-performance.png",
    href: "/products?keyword=ASUS&page=1&pageSize=12",
    cta: "Shop ASUS models",
  },
  {
    eyebrow: "Dell workstations",
    title: "Built for the work that matters.",
    description:
      "Discover Dell desktops, laptops, and workstation setups made for focused, dependable performance.",
    image: "/images/hero-dell-workstation.png",
    href: "/products?keyword=Dell&page=1&pageSize=12",
    cta: "Shop Dell models",
  },
  {
    eyebrow: "Lenovo business",
    title: "Work smarter. Carry less.",
    description:
      "Explore Lenovo laptops designed for dependable performance from the office to wherever work takes you.",
    image: "/images/hero-lenovo-business.png",
    href: "/products?keyword=Lenovo&page=1&pageSize=12",
    cta: "Shop Lenovo models",
  },
  {
    eyebrow: "HP creator systems",
    title: "Create, connect, and do more.",
    description:
      "Discover versatile HP laptops and desktops for creative projects, home offices, and everyday productivity.",
    image: "/images/hero-hp-creator.png",
    href: "/products?keyword=HP&page=1&pageSize=12",
    cta: "Shop HP models",
  },
  {
    eyebrow: "Acer performance",
    title: "Performance for every play.",
    description:
      "Find Acer laptops and displays made for smooth gaming, focused work, and immersive entertainment.",
    image: "/images/hero-acer-performance.png",
    href: "/products?keyword=Acer&page=1&pageSize=12",
    cta: "Shop Acer models",
  },
];

function HardwareHero() {
  const [active, setActive] = useState(0);
  const [paused, setPaused] = useState(false);
  const slide = heroSlides[active];

  useEffect(() => {
    if (paused || window.matchMedia("(prefers-reduced-motion: reduce)").matches)
      return;
    const timer = window.setInterval(
      () => setActive((current) => (current + 1) % heroSlides.length),
      5200,
    );
    return () => window.clearInterval(timer);
  }, [paused]);

  return (
    <section
      className="hero hardware-hero container"
      onMouseEnter={() => setPaused(true)}
      onMouseLeave={() => setPaused(false)}
      onFocusCapture={() => setPaused(true)}
      onBlurCapture={() => setPaused(false)}
      aria-roledescription="carousel"
      aria-label="Featured computer collections"
    >
      <div
        className="hero__backdrop"
        key={slide.image}
        style={{
          backgroundImage: `linear-gradient(90deg, rgba(6,9,13,.98) 0%, rgba(6,9,13,.88) 36%, rgba(6,9,13,.08) 73%), url(${slide.image})`,
        }}
        aria-hidden="true"
      />
      <div className="hero__grid" aria-live="polite">
        <div className="hero__copy">
          <span className="hero__kicker">
            <Sparkles size={16} /> {slide.eyebrow}
          </span>
          <h1>{slide.title}</h1>
          <p>{slide.description}</p>
          <Link className="hero__shop-link" to={slide.href}>
            {slide.cta} <ArrowRight size={17} />
          </Link>
        </div>
        <div aria-hidden="true" />
      </div>
      <div className="hero-carousel__controls">
        <button
          type="button"
          onClick={() =>
            setActive((active - 1 + heroSlides.length) % heroSlides.length)
          }
          aria-label="Previous featured collection"
        >
          <ArrowLeft />
        </button>
        <div>
          {heroSlides.map((item, index) => (
            <button
              type="button"
              className={index === active ? "is-active" : ""}
              onClick={() => setActive(index)}
              aria-label={`Show ${item.eyebrow}`}
              aria-current={index === active ? "true" : undefined}
              key={item.eyebrow}
            />
          ))}
        </div>
        <button
          type="button"
          onClick={() => setActive((active + 1) % heroSlides.length)}
          aria-label="Next featured collection"
        >
          <ArrowRight />
        </button>
      </div>
    </section>
  );
}

function CategoryGlyph({ name }: { name: string }) {
  const value = name.toLowerCase();
  if (value.includes("laptop") || value.includes("notebook")) return <Laptop />;
  if (value.includes("monitor") || value.includes("display"))
    return <Monitor />;
  if (value.includes("keyboard")) return <Keyboard />;
  if (value.includes("mouse")) return <Mouse />;
  if (value.includes("head") || value.includes("audio")) return <Headphones />;
  if (value.includes("memory") || value.includes("ram")) return <MemoryStick />;
  if (
    value.includes("storage") ||
    value.includes("drive") ||
    value.includes("ssd")
  )
    return <HardDrive />;
  if (value.includes("network") || value.includes("router")) return <Network />;
  if (value.includes("gaming") || value.includes("game")) return <Gamepad2 />;
  return <Cpu />;
}

export function HomePage() {
  const categories = useQuery({
    queryKey: catalogKeys.categories,
    queryFn: catalogApi.categories,
  });
  const products = useQuery({
    queryKey: catalogKeys.products({ page: 1, pageSize: 8, featuredOnly: true }),
    queryFn: () =>
      catalogApi.products({ page: 1, pageSize: 8, featuredOnly: true }),
  });
  const storefrontReviews = useQuery({
    queryKey: catalogKeys.storefrontReviews,
    queryFn: () => catalogApi.storefrontReviews(6),
  });
  const activeCategories =
    categories.data?.filter((category) => category.isActive) ?? [];

  return (
    <>
      <HardwareHero />
      <section className="section category-section container">
        <div className="section-heading">
          <div>
            <h2>Shop computer categories</h2>
            <p>Find the right hardware for your next upgrade.</p>
          </div>
          <Link to="/products">
            See all <ArrowRight size={16} />
          </Link>
        </div>
        {categories.isLoading ? (
          <div className="category-grid">
            {Array.from({ length: 6 }, (_, index) => (
              <div className="category-card skeleton" key={index} />
            ))}
          </div>
        ) : categories.isError ? (
          <ErrorState
            error={categories.error}
            onRetry={() => categories.refetch()}
          />
        ) : activeCategories.length ? (
          <div className="category-grid">
            {activeCategories.map((category, index) => (
              <Link
                to={`/categories/${category.id}`}
                className={`category-card category-card--${index % 6}`}
                key={category.id}
              >
                <span className="category-card__visual">
                  {category.imageUrl ? (
                    <img
                      src={resolveApiAsset(category.imageUrl)}
                      alt=""
                    />
                  ) : (
                    <>
                      <CategoryGlyph name={category.name} />
                      <i aria-hidden="true" />
                    </>
                  )}
                </span>
                <div>
                  <h3>{category.name}</h3>
                  <p>{category.description || "Explore the collection"}</p>
                </div>
              </Link>
            ))}
          </div>
        ) : (
          <EmptyState
            title="The shelves are ready"
            description="No categories exist yet. An administrator can create the first one from the catalog workspace."
          />
        )}
      </section>

      <section className="section marketplace-products">
        <div className="container">
          <div className="section-heading">
            <div>
              <h2>Fresh tech in store</h2>
              <p>Hardware selected by the administrator for this collection.</p>
            </div>
            <Link to="/products">
              See all <ArrowRight size={16} />
            </Link>
          </div>
          {products.isLoading ? (
            <SkeletonGrid count={4} />
          ) : products.isError ? (
            <ErrorState
              error={products.error}
              onRetry={() => products.refetch()}
            />
          ) : products.data?.items.length ? (
            <div className="product-grid">
              {products.data.items.map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          ) : (
            <EmptyState
              title="Nothing listed yet"
              description="Products appear here after an administrator publishes them to Fresh Tech."
              action={
                <Link
                  className="button button--secondary button--md"
                  to="/login?redirect=/admin/products"
                >
                  Open admin login
                </Link>
              }
            />
          )}
        </div>
      </section>

      <section className="deal-banner container">
        <div>
          <span className="eyebrow">Power up your setup</span>
          <h2>Your next upgrade starts here.</h2>
          <p>
            Browse the newest computers and components added to the live
            catalog.
          </p>
        </div>
        <Link className="button button--dark button--lg" to="/products">
          Shop all tech <ArrowRight size={17} />
        </Link>
        <div className="deal-banner__art" aria-hidden="true">
          <span>upgrade</span>
          <i />
          <i />
        </div>
      </section>

      <section className="section home-reviews container">
        <div className="section-heading">
          <div>
            <h2>Product reviews</h2>
            <p>Published feedback from customers who bought from the store.</p>
          </div>
        </div>
        {storefrontReviews.isLoading ? (
          <div className="home-review-grid">
            {Array.from({ length: 3 }, (_, index) => (
              <div className="home-review-card skeleton" key={index} />
            ))}
          </div>
        ) : storefrontReviews.isError ? (
          <ErrorState
            error={storefrontReviews.error}
            onRetry={() => storefrontReviews.refetch()}
          />
        ) : storefrontReviews.data?.length ? (
          <div className="home-review-grid">
            {storefrontReviews.data.map((review) => (
              <article className="home-review-card" key={review.id}>
                <div className="home-review-card__rating" aria-label={`${review.rating} out of 5 stars`}>
                  {Array.from({ length: 5 }, (_, index) => (
                    <Star
                      size={15}
                      fill={index < review.rating ? "currentColor" : "none"}
                      key={index}
                    />
                  ))}
                </div>
                <blockquote>“{review.comment}”</blockquote>
                <Link
                  className="home-review-card__product"
                  to={`/products/${review.productId}`}
                >
                  <ProductVisual
                    compact
                    name={review.productName}
                    imageUrl={review.productImageUrl ?? undefined}
                  />
                  <span>
                    <strong>{review.productName}</strong>
                    <small>
                      {review.customerName} · {formatDate(review.createdAt)}
                    </small>
                  </span>
                  <ArrowRight size={16} />
                </Link>
              </article>
            ))}
          </div>
        ) : (
          <EmptyState
            title="Reviews are awaiting publication"
            description="Customer reviews will appear here after an administrator marks them visible."
          />
        )}
      </section>

      <section className="value-strip container">
        <div>
          <span>
            <ShieldCheck />
          </span>
          <h3>Shop with confidence</h3>
          <p>Clear, API-verified product details.</p>
        </div>
        <div>
          <span>
            <Truck />
          </span>
          <h3>Know what&apos;s available</h3>
          <p>Live stock levels before you buy.</p>
        </div>
        <div>
          <span>
            <RotateCcw />
          </span>
          <h3>Simple account tools</h3>
          <p>Orders and saved finds in one place.</p>
        </div>
      </section>
    </>
  );
}

export function ProductsPage({
  forcedCategoryId,
}: { forcedCategoryId?: string } = {}) {
  const [params, setParams] = useSearchParams();
  const keyword = params.get("keyword") ?? "";
  const categoryId = forcedCategoryId ?? params.get("categoryId") ?? "";
  const page = Math.max(1, Number(params.get("page") || 1));
  const pageSize = Number(params.get("pageSize") || 12);
  const [draft, setDraft] = useState(keyword);
  useEffect(() => setDraft(keyword), [keyword]);
  const categories = useQuery({
    queryKey: catalogKeys.categories,
    queryFn: catalogApi.categories,
  });
  const query = {
    keyword,
    categoryId: categoryId || undefined,
    page,
    pageSize,
  };
  const products = useQuery({
    queryKey: catalogKeys.products(query),
    queryFn: () => catalogApi.products(query),
  });
  const update = (values: Record<string, string>) => {
    const next = new URLSearchParams(params);
    Object.entries(values).forEach(([key, value]) =>
      value ? next.set(key, value) : next.delete(key),
    );
    setParams(next);
  };
  const submit = (event: FormEvent) => {
    event.preventDefault();
    update({ keyword: draft, page: "1" });
  };
  const selectedCategory = categories.data?.find((c) => c.id === categoryId);

  return (
    <div className="container listing-page">
      <div className="breadcrumbs">
        <Link to="/">Home</Link>
        <ChevronRight /> <span>{selectedCategory?.name ?? "All products"}</span>
      </div>
      <PageHeader
        eyebrow="Computer store"
        title={
          selectedCategory?.name ??
          (keyword
            ? `Results for “${keyword}”`
            : "Find the right gear for your setup")
        }
        description={
          products.data
            ? `${products.data.totalCount} ${products.data.totalCount === 1 ? "product" : "products"} found`
            : "Search and browse products from the live catalog."
        }
      />
      <form className="listing-toolbar" onSubmit={submit}>
        <div className="listing-search">
          <Search />
          <Input
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
            placeholder="Search the catalog"
          />
          <Button>Search</Button>
        </div>
        <select
          className="input"
          aria-label="Filter by category"
          value={categoryId}
          onChange={(e) => update({ categoryId: e.target.value, page: "1" })}
        >
          <option value="">All categories</option>
          {categories.data
            ?.filter((c) => c.isActive)
            .map((c) => (
              <option value={c.id} key={c.id}>
                {c.name}
              </option>
            ))}
        </select>
        <select
          className="input"
          aria-label="Products per page"
          value={pageSize}
          onChange={(e) => update({ pageSize: e.target.value, page: "1" })}
        >
          <option value="8">8 per page</option>
          <option value="12">12 per page</option>
          <option value="24">24 per page</option>
        </select>
      </form>
      <div className="active-filters">
        {keyword && (
          <button
            onClick={() => {
              setDraft("");
              update({ keyword: "", page: "1" });
            }}
          >
            Search: {keyword} ×
          </button>
        )}
        {selectedCategory && (
          <button onClick={() => update({ categoryId: "", page: "1" })}>
            {selectedCategory.name} ×
          </button>
        )}
      </div>
      {products.isLoading ? (
        <SkeletonGrid count={8} />
      ) : products.isError ? (
        <ErrorState error={products.error} onRetry={() => products.refetch()} />
      ) : products.data?.items.length ? (
        <>
          <div className="product-grid">
            {products.data.items.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
          <nav className="pagination" aria-label="Product pages">
            <Button
              variant="secondary"
              disabled={!products.data.hasPreviousPage}
              onClick={() => update({ page: String(page - 1) })}
            >
              <ArrowLeft size={16} /> Previous
            </Button>
            <span>
              Page <strong>{products.data.page}</strong> of{" "}
              <strong>{Math.max(1, products.data.totalPages)}</strong>
            </span>
            <Button
              variant="secondary"
              disabled={!products.data.hasNextPage}
              onClick={() => update({ page: String(page + 1) })}
            >
              Next <ArrowRight size={16} />
            </Button>
          </nav>
        </>
      ) : (
        <EmptyState
          title="No matching finds"
          description="Try a broader search or clear the current category filter."
          action={
            <Button
              variant="secondary"
              onClick={() => {
                setDraft("");
                setParams({ page: "1", pageSize: String(pageSize) });
              }}
            >
              Clear filters
            </Button>
          }
        />
      )}
    </div>
  );
}

export function CategoryPage() {
  const { categoryId = "" } = useParams();
  const category = useQuery({
    queryKey: ["category", categoryId],
    queryFn: () => catalogApi.category(categoryId),
  });
  if (category.isLoading)
    return (
      <div className="container page-pad">
        <Spinner />
      </div>
    );
  if (category.isError)
    return (
      <div className="container page-pad">
        <ErrorState error={category.error} onRetry={() => category.refetch()} />
      </div>
    );
  return <ProductsPage key={categoryId} forcedCategoryId={categoryId} />;
}

export function ProductDetailPage() {
  const { productId = "" } = useParams();
  const { session } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const queryClient = useQueryClient();
  const [quantity, setQuantity] = useState(1);
  const [reviewRating, setReviewRating] = useState(5);
  const [reviewComment, setReviewComment] = useState("");
  const [reviewSubmitted, setReviewSubmitted] = useState(false);
  const [adminActionMessage, setAdminActionMessage] = useState("");
  const product = useQuery({
    queryKey: catalogKeys.product(productId),
    queryFn: () => catalogApi.product(productId),
  });
  const customerId = session.customerProfileId ?? "";
  const reviews = useQuery({
    queryKey: catalogKeys.reviews(productId),
    queryFn: () => catalogApi.reviews(productId),
    enabled: Boolean(product.data),
  });
  const customerReview = useQuery({
    queryKey: catalogKeys.customerReview(productId, customerId),
    queryFn: () => catalogApi.customerReview(productId, customerId),
    enabled: session.role === "customer" && Boolean(customerId),
  });
  const customerOrders = useQuery({
    queryKey: commerceKeys.orders(customerId),
    queryFn: () => commerceApi.orders(customerId),
    enabled: session.role === "customer" && Boolean(customerId),
  });
  const wishlist = useQuery({
    queryKey: commerceKeys.wishlist(customerId),
    queryFn: () => commerceApi.wishlist(customerId),
    enabled: session.role === "customer" && Boolean(customerId),
  });
  const isSaved = Boolean(
    wishlist.data?.items.some(
      (wishlistItem) => wishlistItem.productId === productId,
    ),
  );
  const addCart = useMutation({
    mutationFn: () => commerceApi.addCart(customerId, productId, quantity),
    onSuccess: (data) =>
      queryClient.setQueryData(commerceKeys.cart(customerId), data),
  });
  const toggleSaved = useMutation({
    mutationFn: () =>
      isSaved
        ? commerceApi.removeWishlist(customerId, productId)
        : commerceApi.addWishlist(customerId, productId),
    onSuccess: (data) =>
      queryClient.setQueryData(commerceKeys.wishlist(customerId), data),
  });
  const eligibleReviewOrder = customerOrders.data?.find(
    (order) =>
      [2, 3, 4, 5].includes(order.status) &&
      order.orderItems.some((orderItem) => orderItem.productId === productId),
  );
  const hasCustomerReview = Boolean(
    reviewSubmitted ||
    customerReview.data,
  );
  const createReview = useMutation({
    mutationFn: () => catalogApi.createReview({
      customerProfileId: customerId,
      productId,
      orderId: eligibleReviewOrder?.id ?? null,
      rating: reviewRating,
      comment: reviewComment.trim() || null,
    }),
    onSuccess: () => {
      setReviewComment("");
      setReviewSubmitted(true);
      queryClient.invalidateQueries({ queryKey: catalogKeys.reviews(productId) });
      queryClient.invalidateQueries({
        queryKey: catalogKeys.customerReview(productId, customerId),
      });
    },
  });
  const deleteReview = useMutation({
    mutationFn: (reviewId: string) => catalogApi.deleteReview(reviewId, customerId),
    onSuccess: () => {
      setReviewSubmitted(false);
      queryClient.invalidateQueries({ queryKey: catalogKeys.reviews(productId) });
      queryClient.invalidateQueries({
        queryKey: catalogKeys.customerReview(productId, customerId),
      });
    },
  });
  const act = (fn: () => void) => {
    if (session.role === "admin") {
      setAdminActionMessage(
        "You are signed in as an administrator. Switch to a customer account to buy or save items.",
      );
      return;
    }
    if (session.isAuthenticated) fn();
    else navigate(`/login?redirect=${encodeURIComponent(location.pathname)}`);
  };
  if (product.isLoading)
    return (
      <div className="container page-pad">
        <Spinner label="Loading product" />
      </div>
    );
  if (product.isError)
    return (
      <div className="container page-pad">
        <ErrorState error={product.error} onRetry={() => product.refetch()} />
      </div>
    );
  if (!product.data) return null;
  const item = product.data;
  return (
    <div className="container product-detail">
      <div className="breadcrumbs">
        <Link to="/">Home</Link>
        <ChevronRight />
        <Link to="/products">Products</Link>
        <ChevronRight />
        <span>{item.name}</span>
      </div>
      <div className="product-detail__grid">
        <div className="product-detail__gallery">
          <ProductVisual name={item.name} imageUrl={item.imageUrls?.[0]} />
          {!item.imageUrls?.length && (
            <span>No product image has been added yet.</span>
          )}
        </div>
        <div className="product-detail__info">
          <span className="eyebrow">Live catalog item</span>
          <h1>{item.name}</h1>
          <a className="product-review-link" href="#reviews">
            <Star size={16} fill="currentColor" />
            {reviews.data?.length ?? 0} {reviews.data?.length === 1 ? "review" : "reviews"}
          </a>
          <p className="product-detail__price">{formatCurrency(item.price)}</p>
          <p className="product-detail__description">{item.description}</p>
          <div className="purchase-panel">
            <label>
              Quantity
              <div className="quantity-control">
                <button onClick={() => setQuantity(Math.max(1, quantity - 1))}>
                  −
                </button>
                <span>{quantity}</span>
                <button
                  onClick={() =>
                    setQuantity(Math.min(item.quantity, quantity + 1))
                  }
                >
                  +
                </button>
              </div>
            </label>
            <Button
              size="lg"
              onClick={() => act(() => addCart.mutate())}
              disabled={
                (item.quantity === 0 && session.role !== "admin") ||
                addCart.isPending
              }
            >
              <ShoppingBag /> {addCart.isPending ? "Adding…" : "Add to cart"}
            </Button>
            <Button
              size="lg"
              variant="secondary"
              className={isSaved ? "is-saved" : undefined}
              onClick={() => act(() => toggleSaved.mutate())}
              disabled={
                toggleSaved.isPending ||
                (session.role === "customer" && wishlist.isLoading)
              }
            >
              <Heart fill={isSaved ? "currentColor" : "none"} />
              {toggleSaved.isPending
                ? isSaved
                  ? "Removing…"
                  : "Saving…"
                : isSaved
                  ? "Remove saved item"
                  : "Save item"}
            </Button>
            {(addCart.error || toggleSaved.error) && (
              <p className="inline-error">
                {(addCart.error ?? toggleSaved.error)?.message}
              </p>
            )}
            {adminActionMessage && (
              <p className="admin-shop-notice" role="status">
                {adminActionMessage}
              </p>
            )}
          </div>
          <div className="product-meta">
            <div>
              <span>Category ID</span>
              <strong>{item.categoryId.slice(0, 8).toUpperCase()}</strong>
            </div>
            <div>
              <span>Added</span>
              <strong>{formatDate(item.createdAt)}</strong>
            </div>
            <div>
              <span>Source</span>
              <strong>Live API</strong>
            </div>
          </div>
        </div>
      </div>
      <section className="reviews-section" id="reviews">
        <div className="section-heading">
          <div>
            <span className="eyebrow">Customer notes</span>
            <h2>Product reviews</h2>
          </div>
          {!session.isAuthenticated && (
            <Link
              className="button button--secondary button--md"
              to={`/login?redirect=${encodeURIComponent(`${location.pathname}#reviews`)}`}
            >
              <Star /> Write a review
            </Link>
          )}
        </div>
        {session.role === "customer" && !customerReview.isLoading && !hasCustomerReview && (
          <Card className="review-form-card">
            <div>
              <span className="eyebrow">
                {eligibleReviewOrder ? "Verified purchase" : "Customer review"}
              </span>
              <h3>Write a review</h3>
              <p>
                Share your experience. Your review will appear after an
                administrator approves it.
              </p>
            </div>
            <form onSubmit={(event) => { event.preventDefault(); createReview.mutate(); }}>
              <div className="review-rating-input" aria-label={`${reviewRating} out of 5 stars`}>
                {Array.from({ length: 5 }, (_, index) => (
                  <button type="button" key={index} onClick={() => setReviewRating(index + 1)} aria-label={`${index + 1} stars`}>
                    <Star fill={index < reviewRating ? "currentColor" : "none"} />
                  </button>
                ))}
              </div>
              <textarea className="input textarea" maxLength={2000} value={reviewComment} onChange={(event) => setReviewComment(event.target.value)} placeholder="What did you like about this product?" />
              {createReview.error && <div className="form-alert">{createReview.error.message}</div>}
              <div className="review-form-actions">
                <Button type="button" variant="ghost" onClick={() => { setReviewComment(""); setReviewRating(5); }}>Clear</Button>
                <Button type="submit" disabled={createReview.isPending}>{createReview.isPending ? "Submitting…" : "Submit review"}</Button>
              </div>
            </form>
          </Card>
        )}
        {session.role === "customer" && hasCustomerReview && <div className="form-success">{reviewSubmitted || customerReview.data?.status !== 1 ? "Your review was submitted and is waiting for administrator approval." : "Your review has been published."}</div>}
        {!session.isAuthenticated && <div className="review-signin"><Link to={`/login?redirect=${encodeURIComponent(`${location.pathname}#reviews`)}`}>Sign in</Link> to write a review.</div>}
        {deleteReview.error && <div className="form-alert">{deleteReview.error.message}</div>}
        {reviews.isLoading ? (
          <Spinner />
        ) : reviews.isError ? (
          <ErrorState error={reviews.error} onRetry={() => reviews.refetch()} />
        ) : reviews.data?.length ? (
          <div className="review-list">
            {reviews.data.map((review) => (
              <Card key={review.id}>
                <div
                  className="stars"
                  aria-label={`${review.rating} out of 5 stars`}
                >
                  {Array.from({ length: 5 }, (_, i) => (
                    <Star
                      key={i}
                      size={16}
                      fill={i < review.rating ? "currentColor" : "none"}
                    />
                  ))}
                </div>
                <p>{review.comment || "No written comment."}</p>
                <footer className="review-card__footer">
                  <small>{formatDate(review.createdAt)}</small>
                  {review.customerProfileId === customerId && (
                    <Button size="sm" variant="ghost" disabled={deleteReview.isPending} onClick={() => { if (window.confirm("Remove your review?")) deleteReview.mutate(review.id); }}><Trash2 /> Remove</Button>
                  )}
                </footer>
              </Card>
            ))}
          </div>
        ) : (
          <EmptyState
            title="No reviews yet"
            description="Verified reviews will appear here when the API returns them."
          />
        )}
      </section>
    </div>
  );
}

/* Legacy in-app credential form retained in history; Keycloak now owns this UI.
const loginSchema = z.object({
  username: z.string().min(1, "Enter your username"),
  password: z.string().min(1, "Enter your password"),
});
type LoginValues = z.infer<typeof loginSchema>;
export function LoginPage() {
  const { login, session } = useAuth();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const [serverError, setServerError] = useState("");
  const [submitted, setSubmitted] = useState(false);
  const form = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { username: "", password: "" },
  });
  const redirect = params.get("redirect") || "/";
  const switchingAccount = params.get("switch") === "1";
  useEffect(() => {
    if (session.isAuthenticated && !switchingAccount && !submitted) {
      navigate(session.role === "admin" ? "/admin" : redirect, { replace: true });
    }
  }, [session.isAuthenticated, session.role, navigate, redirect, switchingAccount, submitted]);
  const submit = form.handleSubmit(async (values) => {
    try {
      setServerError("");
      setSubmitted(true);
      const next = await login(values.username, values.password);
      if (next.role === "admin") {
        const adminTarget = redirect.startsWith("/admin") ? redirect : "/admin";
        navigate("/", { replace: true });
        window.setTimeout(() => navigate(adminTarget), 0);
      } else {
        const customerTarget = redirect.startsWith("/admin") ? "/" : redirect;
        navigate(customerTarget, { replace: true });
      }
    } catch (error) {
      setSubmitted(false);
      setServerError(
        error instanceof Error
          ? error.message
          : "Invalid username or password.",
      );
    }
  });
  return (
    <div className="login-page">
      <div className="login-panel">
        <Link to="/" className="login-back">
          <ArrowLeft /> Back to computer store
        </Link>
        <div className="login-card">
          <span className="brand brand--center">
            <span className="brand__mark">N</span>
            <span>NEXRIG</span>
          </span>
          <span className="pill pill--warm">Development access</span>
          <h1>Welcome back</h1>
          <p>Sign in to continue to your NEXRIG account.</p>
          <form onSubmit={submit}>
            <Field
              label="Username"
              error={form.formState.errors.username?.message}
            >
              <Input autoComplete="username" {...form.register("username")} />
            </Field>
            <Field
              label="Password"
              error={form.formState.errors.password?.message}
            >
              <Input
                type="password"
                autoComplete="current-password"
                {...form.register("password")}
              />
            </Field>
            {serverError && (
              <div className="form-alert" role="alert">
                {serverError}
              </div>
            )}
            <Button size="lg" disabled={form.formState.isSubmitting}>
              {form.formState.isSubmitting ? "Signing in…" : "Sign in"}
            </Button>
          </form>
          <div className="demo-hint">
            <strong>Demo accounts</strong>
            <div>
              <span>Customer</span>
              <code>user / 1234</code>
            </div>
            <div>
              <span>Administrator</span>
              <code>admin / 1234</code>
            </div>
          </div>
          <small>
            Temporary development authentication only. Keycloak and production
            authorization are intentionally not enabled.
          </small>
        </div>
      </div>
      <div className="login-visual">
        <div>
          <span className="eyebrow">One computer store, two views</span>
          <h2>
            Shop simply.
            <br />
            Operate clearly.
          </h2>
          <p>
            The customer and admin experiences share one live backend—without
            fabricated products, orders, or metrics.
          </p>
        </div>
      </div>
    </div>
  );
}
*/

export function LoginPage() {
  const { login, session } = useAuth();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const redirect = params.get("redirect") || "/";
  const switchingAccount = params.get("switch") === "1";

  useEffect(() => {
    if (session.isAuthenticated && !switchingAccount) {
      navigate(session.role === "admin" ? "/admin" : redirect, { replace: true });
      return;
    }

    void login(redirect, switchingAccount);
  }, [session.isAuthenticated, session.role, navigate, redirect, switchingAccount, login]);

  return <div className="auth-loading" role="status">Opening secure sign in...</div>;
}

export function LoginCallbackPage() {
  const { session } = useAuth();
  const navigate = useNavigate();
  const [params] = useSearchParams();

  useEffect(() => {
    if (!session.isAuthenticated) {
      navigate("/login", { replace: true });
      return;
    }

    const requested = params.get("redirect") || "/";
    const target = session.role === "admin"
      ? (requested.startsWith("/admin") ? requested : "/admin")
      : (requested.startsWith("/admin") ? "/" : requested);
    navigate(target, { replace: true });
  }, [session.isAuthenticated, session.role, navigate, params]);

  return <div className="auth-loading" role="status">Finishing secure sign in...</div>;
}

export function RegisterPage() {
  const { register, session } = useAuth();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const redirect = params.get("redirect") || "/account";

  useEffect(() => {
    if (session.isAuthenticated) {
      navigate(session.role === "admin" ? "/admin" : redirect, { replace: true });
      return;
    }

    void register(redirect);
  }, [session.isAuthenticated, session.role, navigate, redirect, register]);

  return <div className="auth-loading" role="status">Opening secure account creation...</div>;
}
