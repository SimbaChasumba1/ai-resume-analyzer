"use client";

import { useEffect, useState } from "react";
import ResumePdfClient from "./ResumePdfClient";

type Props = {
  file: File;
};

export default function LocalResumePreview({ file }: Props) {
  const [url, setUrl] = useState<string>("");

  useEffect(() => {
    const objectUrl = URL.createObjectURL(file);
    setUrl(objectUrl);
    return () => URL.revokeObjectURL(objectUrl);
  }, [file]);

  if (!url) return null;

  return <ResumePdfClient file={url} width={350} />;
}
