import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  AlertTriangle,
  ArrowRight,
  BarChart3,
  Boxes,
  CircleDollarSign,
  Eye,
  EyeOff,
  FolderTree,
  ImagePlus,
  MessageSquareText,
  PackagePlus,
  Pencil,
  Plus,
  RefreshCw,
  RotateCcw,
  Search,
  ShoppingBag,
  Sparkles,
  Trash2,
  UploadCloud,
  X,
} from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import {
  Badge,
  Button,
  Card,
  EmptyState,
  ErrorState,
  Field,
  Input,
  Modal,
  PageHeader,
  Select,
  Spinner,
} from "../components/ui";
import {
  formatCurrency,
  formatDate,
  formatNumber,
  orderStatusLabel,
  paymentStatusLabel,
  productStatusLabel,
  refundStatusLabel,
  shortId,
} from "../core/format";
import type { Category, CustomerProfile, Order, Payment, Product, ProductReview, Refund } from "../core/types";
import { adminApi, adminKeys, type SalesStatsRange } from "../features/admin/api";
import { catalogApi, catalogKeys } from "../features/catalog/api";
import { resolveApiAsset } from "../core/api/apiClient";

export function AdminDashboardPage() {
  const queryClient = useQueryClient();
  const [statsRange, setStatsRange] = useState<SalesStatsRange>("overall");
  const stats = useQuery({
    queryKey: adminKeys.stats(statsRange),
    queryFn: () => adminApi.stats(statsRange),
  });
  const products = useQuery({
    queryKey: catalogKeys.products({ page: 1, pageSize: 5, includeHidden: true }),
    queryFn: () => catalogApi.products({ page: 1, pageSize: 5, includeHidden: true }),
  });
  const categories = useQuery({
    queryKey: adminKeys.categories,
    queryFn: adminApi.categories,
  });
  const refreshDashboard = useMutation({
    mutationFn: () => adminApi.stats(statsRange, true),
    onSuccess: refreshedStats => {
      queryClient.setQueryData(adminKeys.stats(statsRange), refreshedStats);
    },
  });
  const errors = [stats.error, products.error, categories.error].filter(
    Boolean,
  );
  return (
    <>
      <PageHeader
        eyebrow="Overview"
        title="Good afternoon, Admin"
        description="A live operational pulse from the connected backend."
        action={
          <div className="dashboard-actions">
            <label>
              <Select
                aria-label="Dashboard period"
                value={statsRange}
                onChange={event => setStatsRange(event.target.value as SalesStatsRange)}
              >
                <option value="day">A day</option>
                <option value="month">A month</option>
                <option value="year">A year</option>
                <option value="overall">Overall</option>
              </Select>
            </label>
            <Button
              variant="secondary"
              disabled={refreshDashboard.isPending}
              onClick={() => {
                refreshDashboard.mutate();
                products.refetch();
                categories.refetch();
              }}
            >
              <RefreshCw className={refreshDashboard.isPending ? "spin" : undefined} />
              {refreshDashboard.isPending ? "Refreshing..." : "Refresh data"}
            </Button>
          </div>
        }
      />
      {errors.length > 0 && (
        <div className="admin-alert">
          <AlertTriangle /> Some dashboard data is unavailable. Available
          modules remain visible.
        </div>
      )}
      <div className="kpi-grid">
        <Kpi
          icon={<CircleDollarSign />}
          label="Total revenue"
          value={
            stats.isLoading
              ? "…"
              : stats.isError
                ? "Unavailable"
                : formatCurrency(stats.data?.totalRevenue ?? 0)
          }
          note="Paid sales · Redis cached"
        />
        <Kpi
          icon={<Boxes />}
          label="Units sold"
          value={
            stats.isLoading
              ? "…"
              : stats.isError
                ? "Unavailable"
                : formatNumber(stats.data?.totalUnitsSold ?? 0)
          }
          note="Paid units · Redis cached"
        />
        <Kpi
          icon={<ShoppingBag />}
          label="Catalog products"
          value={
            products.isLoading
              ? "…"
              : products.isError
                ? "Unavailable"
                : formatNumber(products.data?.totalCount ?? 0)
          }
          note="Product search API"
        />
        <Kpi
          icon={<FolderTree />}
          label="Categories"
          value={
            categories.isLoading
              ? "…"
              : categories.isError
                ? "Unavailable"
                : formatNumber(categories.data?.length ?? 0)
          }
          note="Category API"
        />
      </div>
      <div className="dashboard-grid">
        <Card className="dashboard-panel">
          <div className="panel-heading">
            <div>
              <span className="eyebrow">Catalog snapshot</span>
              <h2>Recent products</h2>
            </div>
            <Link to="/admin/products" replace>
              Manage <ArrowRight />
            </Link>
          </div>
          {products.isLoading ? (
            <Spinner />
          ) : products.isError ? (
            <ErrorState error={products.error} />
          ) : products.data?.items.length ? (
            <div className="compact-list">
              {products.data.items.map((product) => (
                <Link to={`/products/${product.id}`} key={product.id}>
                  <span className="compact-avatar">
                    {product.name[0]?.toUpperCase()}
                  </span>
                  <div>
                    <strong>{product.name}</strong>
                    <small>
                      {product.quantity} in stock ·{" "}
                      {formatCurrency(product.price)}
                    </small>
                  </div>
                  <Badge tone={product.quantity > 0 ? "success" : "danger"}>
                    {productStatusLabel(product.status)}
                  </Badge>
                </Link>
              ))}
            </div>
          ) : (
            <EmptyState
              title="No products yet"
              description="Start your live catalog by creating the first product."
            />
          )}
        </Card>
        <Card className="dashboard-panel">
          <div className="panel-heading">
            <div>
              <span className="eyebrow">Sales analytics</span>
              <h2>Revenue summary</h2>
            </div>
          </div>
          {stats.isLoading ? (
            <Spinner />
          ) : stats.isError ? (
            <ErrorState error={stats.error} />
          ) : (
            <div className="metric-visual">
              <div className="metric-visual__ring">
                <span>{formatNumber(stats.data?.totalUnitsSold ?? 0)}</span>
                <small>units sold</small>
              </div>
              <div>
                <strong>{formatCurrency(stats.data?.totalRevenue ?? 0)}</strong>
                <span>Lifetime revenue returned by the API</span>
                <p>
                  A time-series chart is intentionally unavailable because the
                  backend currently provides totals only.
                </p>
              </div>
            </div>
          )}
        </Card>
      </div>
      <section className="admin-readiness">
        <span className="eyebrow">Backend capability map</span>
        <h2>Operations at a glance</h2>
        <div>
          <Link to="/admin/categories" replace>
            <FolderTree />
            <span>
              <strong>Categories</strong>
              <small>CRUD ready</small>
            </span>
            <ArrowRight />
          </Link>
          <Link to="/admin/products" replace>
            <ShoppingBag />
            <span>
              <strong>Products</strong>
              <small>CRUD + status ready</small>
            </span>
            <ArrowRight />
          </Link>
          <Link to="/admin/inventory" replace>
            <Boxes />
            <span>
              <strong>Inventory</strong>
              <small>Transactions ready</small>
            </span>
            <ArrowRight />
          </Link>
          <Link to="/admin/orders" replace>
            <BarChart3 />
            <span>
              <strong>Order operations</strong>
              <small>List endpoint required</small>
            </span>
            <ArrowRight />
          </Link>
        </div>
      </section>
    </>
  );
}

function Kpi({
  icon,
  label,
  value,
  note,
}: {
  icon: React.ReactNode;
  label: string;
  value: string;
  note: string;
}) {
  return (
    <Card className="kpi-card">
      <span>{icon}</span>
      <div>
        <small>{label}</small>
        <strong>{value}</strong>
        <p>{note}</p>
      </div>
    </Card>
  );
}

export function AdminCategoriesPage() {
  const qc = useQueryClient();
  const query = useQuery({
    queryKey: adminKeys.categories,
    queryFn: adminApi.categories,
  });
  const [search, setSearch] = useState("");
  const [editing, setEditing] = useState<Category | null | "new">(null);
  const categoryImageInput = useRef<HTMLInputElement>(null);
  const [categoryImageFile, setCategoryImageFile] = useState<File | null>(null);
  const [categoryImagePreview, setCategoryImagePreview] = useState("");
  const [categoryImageError, setCategoryImageError] = useState("");
  const [isDraggingCategoryImage, setIsDraggingCategoryImage] = useState(false);
  const [form, setForm] = useState({ name: "", description: "" });
  const open = (category?: Category) => {
    if (categoryImagePreview) URL.revokeObjectURL(categoryImagePreview);
    setCategoryImageFile(null);
    setCategoryImagePreview("");
    setCategoryImageError("");
    setEditing(category ?? "new");
    setForm({
      name: category?.name ?? "",
      description: category?.description ?? "",
    });
  };
  const selectCategoryImage = (file?: File) => {
    if (!file) return;
    if (!["image/jpeg", "image/png", "image/webp"].includes(file.type)) {
      setCategoryImageError("Use a JPG, PNG, or WebP image.");
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      setCategoryImageError("Category images must be 5 MB or smaller.");
      return;
    }
    if (categoryImagePreview) URL.revokeObjectURL(categoryImagePreview);
    setCategoryImageFile(file);
    setCategoryImagePreview(URL.createObjectURL(file));
    setCategoryImageError("");
  };
  const clearCategoryImage = () => {
    if (categoryImagePreview) URL.revokeObjectURL(categoryImagePreview);
    setCategoryImageFile(null);
    setCategoryImagePreview("");
    setCategoryImageError("");
    if (categoryImageInput.current) categoryImageInput.current.value = "";
  };
  const closeCategoryEditor = () => {
    clearCategoryImage();
    setEditing(null);
  };
  const save = useMutation({
    mutationFn: async () => {
      let categoryId: string;
      if (editing === "new") categoryId = await adminApi.createCategory(form);
      else {
        categoryId = (editing as Category).id;
        await adminApi.updateCategory(categoryId, form);
      }
      if (categoryImageFile)
        await adminApi.uploadCategoryImage(categoryId, categoryImageFile);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: adminKeys.categories });
      qc.invalidateQueries({ queryKey: catalogKeys.categories });
      closeCategoryEditor();
    },
  });
  const remove = useMutation({
    mutationFn: (id: string) => adminApi.deleteCategory(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: adminKeys.categories });
      qc.invalidateQueries({ queryKey: catalogKeys.categories });
    },
  });
  const categoryVisibility = useMutation({
    mutationFn: ({ id, visible }: { id: string; visible: boolean }) =>
      adminApi.setCategoryVisibility(id, visible),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: adminKeys.categories });
      qc.invalidateQueries({ queryKey: catalogKeys.categories });
      qc.invalidateQueries({ queryKey: ["products"] });
    },
  });
  const list =
    query.data?.filter((c) =>
      c.name.toLowerCase().includes(search.toLowerCase()),
    ) ?? [];
  return (
    <>
      <PageHeader
        eyebrow="Catalog"
        title="Categories"
        description="New categories stay hidden until you choose Show in store."
        action={
          <Button onClick={() => open()}>
            <Plus /> Add category
          </Button>
        }
      />
      <div className="admin-toolbar">
        <div>
          <Search />
          <Input
            placeholder="Search categories"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <span>{list.length} results</span>
      </div>
      {query.isLoading ? (
        <Spinner />
      ) : query.isError ? (
        <ErrorState error={query.error} onRetry={() => query.refetch()} />
      ) : list.length ? (
        <div className="table-card">
          <table>
            <thead>
              <tr>
                <th>Category</th>
                <th>Description</th>
                <th>Status</th>
                <th>Updated</th>
                <th>
                  <span className="sr-only">Actions</span>
                </th>
              </tr>
            </thead>
            <tbody>
              {list.map((category) => (
                <tr key={category.id}>
                  <td>
                    <div className="category-table-name">
                      <span className="category-table-thumb">
                        {category.imageUrl ? (
                          <img src={resolveApiAsset(category.imageUrl)} alt="" />
                        ) : (
                          <ImagePlus aria-hidden="true" />
                        )}
                      </span>
                      <span>
                        <strong>{category.name}</strong>
                        <small>{shortId(category.id)}</small>
                      </span>
                    </div>
                  </td>
                  <td>
                    {category.description || (
                      <span className="muted">No description</span>
                    )}
                  </td>
                  <td>
                    <Badge tone={category.isActive ? "success" : "neutral"}>
                      {category.isActive ? "Active" : "Inactive"}
                    </Badge>
                  </td>
                  <td>
                    {formatDate(category.updatedAt || category.createdAt)}
                  </td>
                  <td>
                    <div className="row-actions">
                      <button
                        onClick={() => open(category)}
                        aria-label={`Edit ${category.name}`}
                      >
                        <Pencil />
                      </button>
                      <button
                        onClick={() =>
                          categoryVisibility.mutate({
                            id: category.id,
                            visible: !category.isActive,
                          })
                        }
                        disabled={categoryVisibility.isPending}
                        aria-label={`${category.isActive ? "Hide" : "Show"} ${category.name} in store`}
                        title={category.isActive ? "Hide from store" : "Show in store"}
                      >
                        {category.isActive ? <EyeOff /> : <Eye />}
                      </button>
                      <button
                        onClick={() => {
                          if (window.confirm(`Delete ${category.name}?`))
                            remove.mutate(category.id);
                        }}
                        aria-label={`Delete ${category.name}`}
                      >
                        <Trash2 />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <EmptyState
          title="No categories found"
          description={
            search
              ? "Try another search."
              : "Create the first category to begin organizing the catalog."
          }
          action={
            !search ? (
              <Button onClick={() => open()}>
                <Plus /> Add category
              </Button>
            ) : undefined
          }
        />
      )}{" "}
      {editing && (
        <Modal
          title={editing === "new" ? "Add category" : "Edit category"}
          onClose={closeCategoryEditor}
        >
          <form
            className="modal-form"
            onSubmit={(e) => {
              e.preventDefault();
              save.mutate();
            }}
          >
            <div className="product-editor__image category-editor__image">
              <div className="product-editor__image-heading">
                <div>
                  <strong>Category image</strong>
                  <span>Shown as a small circle on the storefront.</span>
                </div>
                <span>Optional</span>
              </div>
              <input
                ref={categoryImageInput}
                className="sr-only"
                type="file"
                accept="image/jpeg,image/png,image/webp"
                onChange={(event) =>
                  selectCategoryImage(event.target.files?.[0])
                }
              />
              <div
                className={`image-dropzone${isDraggingCategoryImage ? " is-dragging" : ""}${categoryImagePreview || (editing !== "new" && editing.imageUrl) ? " has-image" : ""}`}
                onDragEnter={(event) => {
                  event.preventDefault();
                  setIsDraggingCategoryImage(true);
                }}
                onDragOver={(event) => event.preventDefault()}
                onDragLeave={(event) => {
                  if (!event.currentTarget.contains(event.relatedTarget as Node))
                    setIsDraggingCategoryImage(false);
                }}
                onDrop={(event) => {
                  event.preventDefault();
                  setIsDraggingCategoryImage(false);
                  selectCategoryImage(event.dataTransfer.files?.[0]);
                }}
              >
                {categoryImagePreview ||
                (editing !== "new" && editing.imageUrl) ? (
                  <>
                    <img
                      src={
                        categoryImagePreview ||
                        resolveApiAsset(
                          editing !== "new"
                            ? editing.imageUrl ?? undefined
                            : undefined,
                        )
                      }
                      alt="Category preview"
                    />
                    <div className="image-dropzone__overlay">
                      <Button
                        type="button"
                        size="sm"
                        variant="secondary"
                        onClick={() => categoryImageInput.current?.click()}
                      >
                        <ImagePlus /> Replace image
                      </Button>
                      {categoryImagePreview && (
                        <button
                          type="button"
                          className="image-remove"
                          onClick={clearCategoryImage}
                          aria-label="Remove selected image"
                        >
                          <X />
                        </button>
                      )}
                    </div>
                  </>
                ) : (
                  <button
                    type="button"
                    className="image-dropzone__empty"
                    onClick={() => categoryImageInput.current?.click()}
                  >
                    <span>
                      <UploadCloud />
                    </span>
                    <strong>Drop a category image here</strong>
                    <small>
                      or click to browse · JPG, PNG or WebP · up to 5 MB
                    </small>
                  </button>
                )}
              </div>
              {categoryImageError && (
                <p className="field__error">{categoryImageError}</p>
              )}
            </div>
            <Field label="Category name">
              <Input
                required
                maxLength={100}
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
              />
            </Field>
            <Field label="Description">
              <textarea
                className="input textarea"
                value={form.description}
                onChange={(e) =>
                  setForm({ ...form, description: e.target.value })
                }
              />
            </Field>
            {save.error && (
              <div className="form-alert">{save.error.message}</div>
            )}
            <div className="form-actions">
              <Button
                type="button"
                variant="secondary"
                onClick={closeCategoryEditor}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={save.isPending}>
                {save.isPending
                  ? "Saving…"
                  : categoryImageFile
                    ? "Save category & image"
                    : "Save category"}
              </Button>
            </div>
          </form>
        </Modal>
      )}
    </>
  );
}

export function AdminProductsPage() {
  const qc = useQueryClient();
  const [keyword, setKeyword] = useState("");
  const [draft, setDraft] = useState("");
  const [page, setPage] = useState(1);
  const products = useQuery({
    queryKey: catalogKeys.products({ keyword, page, pageSize: 12, includeHidden: true }),
    queryFn: () => catalogApi.products({ keyword, page, pageSize: 12, includeHidden: true }),
  });
  const categories = useQuery({
    queryKey: adminKeys.categories,
    queryFn: adminApi.categories,
  });
  const [editing, setEditing] = useState<Product | null | "new">(null);
  const imageInput = useRef<HTMLInputElement>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState("");
  const [imageError, setImageError] = useState("");
  const [isDraggingImage, setIsDraggingImage] = useState(false);
  const [form, setForm] = useState({
    categoryId: "",
    name: "",
    description: "",
    price: 0,
    quantity: 0,
  });
  const open = (p?: Product) => {
    if (imagePreview) URL.revokeObjectURL(imagePreview);
    setImageFile(null);
    setImagePreview("");
    setImageError("");
    setEditing(p ?? "new");
    setForm({
      categoryId: p?.categoryId ?? categories.data?.[0]?.id ?? "",
      name: p?.name ?? "",
      description: p?.description ?? "",
      price: p?.price ?? 0,
      quantity: p?.quantity ?? 0,
    });
  };
  const selectImage = (file?: File) => {
    if (!file) return;
    if (!["image/jpeg", "image/png", "image/webp"].includes(file.type)) {
      setImageError("Use a JPG, PNG, or WebP image.");
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      setImageError("Product images must be 5 MB or smaller.");
      return;
    }
    if (imagePreview) URL.revokeObjectURL(imagePreview);
    setImageFile(file);
    setImagePreview(URL.createObjectURL(file));
    setImageError("");
  };
  const clearSelectedImage = () => {
    if (imagePreview) URL.revokeObjectURL(imagePreview);
    setImageFile(null);
    setImagePreview("");
    setImageError("");
    if (imageInput.current) imageInput.current.value = "";
  };
  const closeEditor = () => {
    clearSelectedImage();
    setEditing(null);
  };
  const invalidate = () => qc.invalidateQueries({ queryKey: ["products"] });
  const save = useMutation({
    mutationFn: async () => {
      let productId: string;
      if (editing === "new") productId = await adminApi.createProduct(form);
      else {
        productId = (editing as Product).id;
        await adminApi.updateProduct(productId, form);
      }
      if (imageFile) await adminApi.uploadProductImage(productId, imageFile);
    },
    onSuccess: () => {
      invalidate();
      closeEditor();
    },
  });
  const status = useMutation({
    mutationFn: ({ id, active }: { id: string; active: boolean }) =>
      active ? adminApi.activateProduct(id) : adminApi.deactivateProduct(id),
    onSuccess: invalidate,
  });
  const freshTechVisibility = useMutation({
    mutationFn: ({ id, visible }: { id: string; visible: boolean }) =>
      adminApi.setFreshTechVisibility(id, visible),
    onSuccess: invalidate,
  });
  const remove = useMutation({
    mutationFn: (id: string) => adminApi.deleteProduct(id),
    onSuccess: invalidate,
  });
  return (
    <>
      <PageHeader
        eyebrow="Catalog"
        title="Products"
        description="New products stay hidden. Publish them to the store and Fresh Tech separately."
        action={
          <Button onClick={() => open()} disabled={!categories.data?.length}>
            <PackagePlus /> Add product
          </Button>
        }
      />
      <form
        className="admin-toolbar"
        onSubmit={(e) => {
          e.preventDefault();
          setKeyword(draft);
          setPage(1);
        }}
      >
        <div>
          <Search />
          <Input
            placeholder="Search products"
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
          />
          <Button size="sm">Search</Button>
        </div>
        <span>{products.data?.totalCount ?? 0} products</span>
      </form>
      {categories.data?.length === 0 && (
        <div className="admin-alert">
          <AlertTriangle /> Create a category before adding products.
        </div>
      )}
      {products.isLoading ? (
        <Spinner />
      ) : products.isError ? (
        <ErrorState error={products.error} onRetry={() => products.refetch()} />
      ) : products.data?.items.length ? (
        <>
          <div className="table-card">
            <table>
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Category</th>
                  <th>Price</th>
                  <th>Stock</th>
                  <th>Storefront</th>
                  <th>Fresh tech</th>
                  <th>Updated</th>
                  <th>
                    <span className="sr-only">Actions</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {products.data.items.map((product) => (
                  <tr key={product.id}>
                    <td>
                      <strong>{product.name}</strong>
                      <small>{shortId(product.id)}</small>
                    </td>
                    <td>
                      {categories.data?.find((c) => c.id === product.categoryId)
                        ?.name ?? shortId(product.categoryId)}
                    </td>
                    <td>{formatCurrency(product.price)}</td>
                    <td>
                      <Badge
                        tone={
                          product.quantity > 5
                            ? "success"
                            : product.quantity > 0
                              ? "warning"
                              : "danger"
                        }
                      >
                        {product.quantity}
                      </Badge>
                    </td>
                    <td>
                      <Badge tone={product.status === 1 ? "success" : "neutral"}>
                        {product.status === 1 ? "Visible" : "Hidden"}
                      </Badge>
                    </td>
                    <td>
                      <Badge tone={product.isFeatured ? "success" : "neutral"}>
                        {product.isFeatured ? "Visible" : "Hidden"}
                      </Badge>
                    </td>
                    <td>
                      {formatDate(product.updatedAt || product.createdAt)}
                    </td>
                    <td>
                      <div className="row-actions">
                        <button
                          onClick={() => open(product)}
                          aria-label={`Edit ${product.name}`}
                        >
                          <Pencil />
                        </button>
                        <button
                          onClick={() =>
                            status.mutate({
                              id: product.id,
                              active: product.status !== 1,
                            })
                          }
                          aria-label={`${product.status === 1 ? "Hide" : "Show"} ${product.name} in store`}
                          title={product.status === 1 ? "Hide from store" : "Show in store"}
                        >
                          {product.status === 1 ? <EyeOff /> : <Eye />}
                        </button>
                        <button
                          onClick={() =>
                            freshTechVisibility.mutate({
                              id: product.id,
                              visible: !product.isFeatured,
                            })
                          }
                          disabled={freshTechVisibility.isPending}
                          aria-label={`${product.isFeatured ? "Remove" : "Add"} ${product.name} ${product.isFeatured ? "from" : "to"} Fresh Tech`}
                          title={product.isFeatured ? "Hide from Fresh Tech" : "Show in Fresh Tech"}
                        >
                          <Sparkles />
                        </button>
                        <button
                          onClick={() => {
                            if (window.confirm(`Delete ${product.name}?`))
                              remove.mutate(product.id);
                          }}
                        >
                          <Trash2 />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="pagination">
            <Button
              variant="secondary"
              disabled={!products.data.hasPreviousPage}
              onClick={() => setPage(page - 1)}
            >
              Previous
            </Button>
            <span>
              Page {page} of {Math.max(1, products.data.totalPages)}
            </span>
            <Button
              variant="secondary"
              disabled={!products.data.hasNextPage}
              onClick={() => setPage(page + 1)}
            >
              Next
            </Button>
          </div>
        </>
      ) : (
        <EmptyState
          title="No products found"
          description={
            keyword
              ? "Try a broader search."
              : "Create the first product after adding a category."
          }
          action={
            !keyword && categories.data?.length ? (
              <Button onClick={() => open()}>
                <Plus /> Add product
              </Button>
            ) : undefined
          }
        />
      )}{" "}
      {editing && (
        <Modal
          title={editing === "new" ? "Add product" : "Edit product"}
          onClose={closeEditor}
        >
          <form
            className="modal-form"
            onSubmit={(e) => {
              e.preventDefault();
              save.mutate();
            }}
          >
            <div className="product-editor__image">
              <div className="product-editor__image-heading">
                <div><strong>Product image</strong><span>Make the hardware easy to recognize at a glance.</span></div>
                <span>Optional</span>
              </div>
              <input ref={imageInput} className="sr-only" type="file" accept="image/jpeg,image/png,image/webp" onChange={(event) => selectImage(event.target.files?.[0])} />
              <div
                className={`image-dropzone${isDraggingImage ? " is-dragging" : ""}${imagePreview || (editing !== "new" && editing.imageUrls?.[0]) ? " has-image" : ""}`}
                onDragEnter={(event) => { event.preventDefault(); setIsDraggingImage(true); }}
                onDragOver={(event) => event.preventDefault()}
                onDragLeave={(event) => { if (!event.currentTarget.contains(event.relatedTarget as Node)) setIsDraggingImage(false); }}
                onDrop={(event) => { event.preventDefault(); setIsDraggingImage(false); selectImage(event.dataTransfer.files?.[0]); }}
              >
                {imagePreview || (editing !== "new" && editing.imageUrls?.[0]) ? <>
                  <img src={imagePreview || resolveApiAsset(editing !== "new" ? editing.imageUrls?.[0] : undefined)} alt="Product preview" />
                  <div className="image-dropzone__overlay">
                    <Button type="button" size="sm" variant="secondary" onClick={() => imageInput.current?.click()}><ImagePlus /> Replace image</Button>
                    {imagePreview && <button type="button" className="image-remove" onClick={clearSelectedImage} aria-label="Remove selected image"><X /></button>}
                  </div>
                </> : <button type="button" className="image-dropzone__empty" onClick={() => imageInput.current?.click()}><span><UploadCloud /></span><strong>Drop a product image here</strong><small>or click to browse · JPG, PNG or WebP · up to 5 MB</small></button>}
              </div>
              {imageError && <p className="field__error">{imageError}</p>}
            </div>
            <Field label="Category">
              <Select
                required
                value={form.categoryId}
                onChange={(e) =>
                  setForm({ ...form, categoryId: e.target.value })
                }
              >
                <option value="">Select category</option>
                {categories.data?.map((c) => (
                  <option value={c.id} key={c.id}>
                    {c.name}
                  </option>
                ))}
              </Select>
            </Field>
            <Field label="Product name">
              <Input
                required
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
              />
            </Field>
            <Field label="Description">
              <textarea
                required
                className="input textarea"
                value={form.description}
                onChange={(e) =>
                  setForm({ ...form, description: e.target.value })
                }
              />
            </Field>
            <div className="form-grid">
              <Field label="Price">
                <Input
                  type="number"
                  min="0.01"
                  step="0.01"
                  required
                  value={form.price}
                  onChange={(e) =>
                    setForm({ ...form, price: Number(e.target.value) })
                  }
                />
              </Field>
              <Field label="Initial quantity">
                <Input
                  type="number"
                  min="0"
                  required
                  value={form.quantity}
                  onChange={(e) =>
                    setForm({ ...form, quantity: Number(e.target.value) })
                  }
                />
              </Field>
            </div>
            {save.error && (
              <div className="form-alert">{save.error.message}</div>
            )}
            <div className="form-actions">
              <Button
                type="button"
                variant="secondary"
                onClick={closeEditor}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={save.isPending}>
                {save.isPending ? "Saving…" : imageFile ? "Save product & image" : "Save product"}
              </Button>
            </div>
          </form>
        </Modal>
      )}
    </>
  );
}

export function AdminInventoryPage() {
  const products = useQuery({
    queryKey: catalogKeys.products({ page: 1, pageSize: 100, includeHidden: true }),
    queryFn: () => catalogApi.products({ page: 1, pageSize: 100, includeHidden: true }),
  });
  const [productId, setProductId] = useState("");
  useEffect(() => {
    if (!productId && products.data?.items[0])
      setProductId(products.data.items[0].id);
  }, [products.data, productId]);
  const history = useQuery({
    queryKey: adminKeys.inventory(productId),
    queryFn: () => adminApi.inventory(productId),
    enabled: Boolean(productId),
  });
  const qc = useQueryClient();
  const [form, setForm] = useState({ type: 1, quantity: 1, note: "" });
  const create = useMutation({
    mutationFn: () =>
      adminApi.createInventory(productId, form.type, form.quantity, form.note),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: adminKeys.inventory(productId) });
      qc.invalidateQueries({ queryKey: ["products"] });
    },
  });
  const selected = products.data?.items.find((p) => p.id === productId);
  return (
    <>
      <PageHeader
        eyebrow="Operations"
        title="Inventory"
        description="Record stock movement as persisted inventory transactions."
      />
      <div className="inventory-layout">
        <Card className="inventory-control">
          <span className="eyebrow">Stock operation</span>
          <h2>Adjust inventory</h2>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              create.mutate();
            }}
          >
            <Field label="Product">
              <Select
                value={productId}
                onChange={(e) => setProductId(e.target.value)}
                required
              >
                <option value="">Select product</option>
                {products.data?.items.map((p) => (
                  <option value={p.id} key={p.id}>
                    {p.name}
                  </option>
                ))}
              </Select>
            </Field>
            {selected && (
              <div className="stock-callout">
                <span>Current API quantity</span>
                <strong>{selected.quantity}</strong>
              </div>
            )}
            <Field label="Transaction type">
              <Select
                value={form.type}
                onChange={(e) =>
                  setForm({ ...form, type: Number(e.target.value) })
                }
              >
                <option value="1">Stock in</option>
                <option value="2">Stock out</option>
                <option value="3">Adjustment</option>
              </Select>
            </Field>
            <Field label="Quantity">
              <Input
                type="number"
                min="1"
                required
                value={form.quantity}
                onChange={(e) =>
                  setForm({ ...form, quantity: Number(e.target.value) })
                }
              />
            </Field>
            <Field label="Reason / note">
              <Input
                value={form.note}
                onChange={(e) => setForm({ ...form, note: e.target.value })}
              />
            </Field>
            {create.error && (
              <div className="form-alert">{create.error.message}</div>
            )}
            {create.isSuccess && (
              <div className="form-success">Inventory transaction saved.</div>
            )}
            <Button disabled={!productId || create.isPending}>
              {create.isPending ? "Recording…" : "Record transaction"}
            </Button>
          </form>
        </Card>
        <Card className="inventory-history">
          <div className="panel-heading">
            <div>
              <span className="eyebrow">Audit trail</span>
              <h2>Transaction history</h2>
            </div>
          </div>
          {history.isLoading ? (
            <Spinner />
          ) : history.isError ? (
            <ErrorState
              error={history.error}
              onRetry={() => history.refetch()}
            />
          ) : history.data?.length ? (
            <div className="timeline-list">
              {history.data.map((tx) => (
                <div key={tx.id}>
                  <span className={`timeline-dot timeline-dot--${tx.type}`} />
                  <div>
                    <strong>
                      {({ 1: "Stock in", 2: "Stock out", 3: "Adjustment" } as Record<number, string>)[tx.type] ??
                        `Type ${tx.type}`}
                    </strong>
                    <p>{tx.note || "No note provided"}</p>
                    <small>
                      {formatDate(tx.createdAt)} · {shortId(tx.id)}
                    </small>
                  </div>
                  <b>
                    {tx.type === 2 ? "−" : "+"}
                    {tx.quantity}
                  </b>
                </div>
              ))}
            </div>
          ) : (
            <EmptyState
              title="No inventory history"
              description={
                productId
                  ? "Record the first stock transaction for this product."
                  : "Choose a product to view its inventory history."
              }
            />
          )}
        </Card>
      </div>
    </>
  );
}

export function AdminUnavailablePage({
  title,
  description,
}: {
  title: string;
  description: string;
}) {
  return (
    <>
      <PageHeader
        eyebrow="Backend contract required"
        title={title}
        description={description}
      />
      <EmptyState
        title={`${title} is intentionally unavailable`}
        description="The current backend does not expose the list, filter, and mutation contracts this operational screen needs. No placeholder rows or fake metrics have been inserted."
        action={
          <Link className="button button--secondary button--md" to="/admin" replace>
            Return to dashboard
          </Link>
        }
      />
    </>
  );
}

function statusTone(status: number, success: number[], danger: number[]) {
  if (success.includes(status)) return "success" as const;
  if (danger.includes(status)) return "danger" as const;
  return "info" as const;
}

export function AdminOrdersPage() {
  const query = useQuery({ queryKey: adminKeys.orders, queryFn: adminApi.orders });
  return <>
    <PageHeader eyebrow="Operations" title="Orders" description="Every persisted customer order, separated by its real backend status." />
    {query.isLoading ? <Spinner /> : query.isError ? <ErrorState error={query.error} onRetry={() => query.refetch()} /> : query.data?.length ? <div className="table-card"><table><thead><tr><th>Order</th><th>Customer</th><th>Status</th><th>Items</th><th>Total</th><th>Placed</th></tr></thead><tbody>{query.data.map((order: Order) => <tr key={order.id}><td><strong>{shortId(order.id)}</strong></td><td><small>{shortId(order.customerProfileId)}</small></td><td><Badge tone={statusTone(order.status, [2, 3, 4, 5], [6, 8])}>{orderStatusLabel(order.status)}</Badge></td><td>{order.orderItems.length}</td><td><strong>{formatCurrency(order.totalAmount)}</strong></td><td>{formatDate(order.createdAt)}</td></tr>)}</tbody></table></div> : <EmptyState title="No orders" description="Placed customer orders will appear here." />}
  </>;
}

export function AdminPaymentsPage() {
  const query = useQuery({ queryKey: adminKeys.payments, queryFn: adminApi.payments });
  const payments = query.data ?? [];
  return <>
    <PageHeader eyebrow="Operations" title="Payments" description="Paid, failed, pending, and refunded payments are shown as separate persisted states." />
    <div className="kpi-grid operations-kpis"><Kpi icon={<CircleDollarSign />} label="Paid" value={formatNumber(payments.filter((p) => p.status === 2).length)} note="Completed payments" /><Kpi icon={<AlertTriangle />} label="Failed" value={formatNumber(payments.filter((p) => p.status === 3).length)} note="Declined tests" /><Kpi icon={<RotateCcw />} label="Refunded" value={formatNumber(payments.filter((p) => p.status === 4).length)} note="Completed refunds" /></div>
    {query.isLoading ? <Spinner /> : query.isError ? <ErrorState error={query.error} onRetry={() => query.refetch()} /> : payments.length ? <div className="table-card"><table><thead><tr><th>Payment</th><th>Order</th><th>Status</th><th>Method</th><th>Amount</th><th>Created</th></tr></thead><tbody>{payments.map((payment: Payment) => <tr key={payment.id}><td><strong>{shortId(payment.id)}</strong></td><td><small>{shortId(payment.orderId)}</small></td><td><Badge tone={statusTone(payment.status, [2, 4], [3])}>{paymentStatusLabel(payment.status)}</Badge></td><td>{payment.paymentMethod}</td><td><strong>{formatCurrency(payment.amount)}</strong></td><td>{formatDate(payment.createdAt)}</td></tr>)}</tbody></table></div> : <EmptyState title="No payments" description="Checkout payment attempts will appear here." />}
  </>;
}

export function AdminRefundsPage() {
  const qc = useQueryClient();
  const query = useQuery({ queryKey: adminKeys.refunds, queryFn: adminApi.refunds });
  const approve = useMutation({ mutationFn: adminApi.approveRefund, onSuccess: () => { qc.invalidateQueries({ queryKey: adminKeys.refunds }); qc.invalidateQueries({ queryKey: adminKeys.payments }); } });
  return <>
    <PageHeader eyebrow="Operations" title="Refunds" description="Refund requests are kept separate from failed or declined payments." />
    {approve.error && <div className="form-alert">{approve.error.message}</div>}
    {query.isLoading ? <Spinner /> : query.isError ? <ErrorState error={query.error} onRetry={() => query.refetch()} /> : query.data?.length ? <div className="table-card"><table><thead><tr><th>Refund</th><th>Payment</th><th>Status</th><th>Reason</th><th>Amount</th><th>Action</th></tr></thead><tbody>{query.data.map((refund: Refund) => <tr key={refund.id}><td><strong>{shortId(refund.id)}</strong><small>{formatDate(refund.createdAt)}</small></td><td><small>{shortId(refund.paymentId)}</small></td><td><Badge tone={statusTone(refund.status, [2, 4], [3])}>{refundStatusLabel(refund.status)}</Badge></td><td>{refund.reason}</td><td><strong>{formatCurrency(refund.amount)}</strong></td><td>{refund.status === 1 ? <Button size="sm" disabled={approve.isPending} onClick={() => approve.mutate(refund.id)}>Approve</Button> : <span className="muted">Processed</span>}</td></tr>)}</tbody></table></div> : <EmptyState title="No refunds" description="Declined payments do not create refunds. Genuine refund requests will appear here." />}
  </>;
}

export function AdminReportsPage() {
  const orders = useQuery({ queryKey: adminKeys.orders, queryFn: adminApi.orders });
  const payments = useQuery({ queryKey: adminKeys.payments, queryFn: adminApi.payments });
  const refunds = useQuery({ queryKey: adminKeys.refunds, queryFn: adminApi.refunds });
  const loading = orders.isLoading || payments.isLoading || refunds.isLoading;
  const error = orders.error || payments.error || refunds.error;
  const paid = payments.data?.filter((payment) => payment.status === 2) ?? [];
  return <>
    <PageHeader eyebrow="Reports" title="Commerce report" description="Live totals calculated from persisted orders, payments, and refunds." action={<Button variant="secondary" onClick={() => { orders.refetch(); payments.refetch(); refunds.refetch(); }}><RefreshCw /> Refresh data</Button>} />
    {loading ? <Spinner /> : error ? <ErrorState error={error} /> : <><div className="kpi-grid"><Kpi icon={<ShoppingBag />} label="Orders" value={formatNumber(orders.data?.length ?? 0)} note="All order states" /><Kpi icon={<CircleDollarSign />} label="Paid revenue" value={formatCurrency(paid.reduce((total, payment) => total + payment.amount, 0))} note={`${paid.length} successful payments`} /><Kpi icon={<AlertTriangle />} label="Declined" value={formatNumber(payments.data?.filter((payment) => payment.status === 3).length ?? 0)} note="Failed payments only" /><Kpi icon={<RotateCcw />} label="Refund requests" value={formatNumber(refunds.data?.length ?? 0)} note="Tracked separately" /></div><Card className="dashboard-panel"><div className="panel-heading"><div><span className="eyebrow">Status breakdown</span><h2>Payment results</h2></div></div><div className="compact-list report-status-list">{[1, 2, 3, 4].map((status) => <div key={status}><span className="compact-avatar">{payments.data?.filter((payment) => payment.status === status).length ?? 0}</span><div><strong>{paymentStatusLabel(status)}</strong><small>Persisted payment records</small></div><Badge tone={statusTone(status, [2, 4], [3])}>{paymentStatusLabel(status)}</Badge></div>)}</div></Card></>}
  </>;
}

export function AdminCustomersPage() {
  const query = useQuery({ queryKey: adminKeys.customers, queryFn: adminApi.customers });
  return <>
    <PageHeader eyebrow="Customers" title="Customer management" description="Customer profiles loaded directly from the database." />
    {query.isLoading ? <Spinner /> : query.isError ? <ErrorState error={query.error} onRetry={() => query.refetch()} /> : query.data?.length ? <div className="table-card"><table><thead><tr><th>Customer</th><th>Email</th><th>Phone</th><th>Status</th><th>Joined</th></tr></thead><tbody>{query.data.map((customer: CustomerProfile) => <tr key={customer.id}><td><strong>{customer.firstName} {customer.lastName}</strong><small>{shortId(customer.id)}</small></td><td>{customer.email}</td><td>{customer.phone || <span className="muted">Not provided</span>}</td><td><Badge tone={customer.isActive ? "success" : "neutral"}>{customer.isActive ? "Active" : "Inactive"}</Badge></td><td>{formatDate(customer.createdAt)}</td></tr>)}</tbody></table></div> : <EmptyState title="No customers" description="Registered customer profiles will appear here." />}
  </>;
}

export function AdminReviewsPage() {
  const qc = useQueryClient();
  const reviews = useQuery({ queryKey: adminKeys.reviews, queryFn: adminApi.reviews });
  const customers = useQuery({ queryKey: adminKeys.customers, queryFn: adminApi.customers });
  const products = useQuery({ queryKey: catalogKeys.products({ page: 1, pageSize: 100, includeHidden: true }), queryFn: () => catalogApi.products({ page: 1, pageSize: 100, includeHidden: true }) });
  const visibility = useMutation({ mutationFn: ({ id, visible }: { id: string; visible: boolean }) => adminApi.setReviewVisibility(id, visible), onSuccess: (updated) => { qc.setQueryData<ProductReview[]>(adminKeys.reviews, (current = []) => current.map((review) => review.id === updated.id ? updated : review)); qc.invalidateQueries({ queryKey: catalogKeys.reviews(updated.productId) }); qc.invalidateQueries({ queryKey: catalogKeys.storefrontReviews }); } });
  const customerNames = new Map((customers.data ?? []).map((customer) => [customer.id, `${customer.firstName} ${customer.lastName}`.trim()]));
  const productNames = new Map((products.data?.items ?? []).map((product) => [product.id, product.name]));
  return <>
    <PageHeader eyebrow="Customers" title="Review moderation" description="All submitted product reviews, including reviews hidden from the storefront." />
    {visibility.error && <div className="form-alert">{visibility.error.message}</div>}
    {reviews.isLoading || customers.isLoading || products.isLoading ? <Spinner /> : reviews.isError ? <ErrorState error={reviews.error} onRetry={() => reviews.refetch()} /> : reviews.data?.length ? <div className="table-card"><table><thead><tr><th>Product</th><th>Customer</th><th>Rating</th><th>Review</th><th>Status</th><th>Action</th></tr></thead><tbody>{reviews.data.map((review: ProductReview) => <tr key={review.id}><td><strong>{productNames.get(review.productId) || shortId(review.productId)}</strong></td><td>{customerNames.get(review.customerProfileId) || shortId(review.customerProfileId)}</td><td><span className="admin-rating">{review.rating} / 5</span></td><td>{review.comment || <span className="muted">No comment</span>}<small>{formatDate(review.createdAt)}</small></td><td><Badge tone={review.status === 1 ? "success" : "neutral"}>{review.status === 1 ? "Visible" : "Hidden"}</Badge></td><td><Button size="sm" variant="secondary" disabled={visibility.isPending} onClick={() => visibility.mutate({ id: review.id, visible: review.status !== 1 })}>{review.status === 1 ? <><EyeOff /> Hide</> : <><Eye /> Show</>}</Button></td></tr>)}</tbody></table></div> : <EmptyState title="No reviews" description="Customer reviews submitted from product pages will appear here." action={<Link className="button button--secondary button--md" to="/products"><MessageSquareText /> Open storefront products</Link>} />}
  </>;
}
